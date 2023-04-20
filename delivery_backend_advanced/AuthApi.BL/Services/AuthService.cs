using System.Security.Claims;
using AuthApi.Common.Dtos;
using AuthApi.Common.Enums;
using AuthApi.Common.Interfaces;
using AuthApi.DAL;
using AuthApi.DAL.Entities;
using AutoMapper;
using Common.Configurations;
using delivery_backend_advanced.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.BL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AuthDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public AuthService(AuthDbContext context, UserManager<AppUser> userManager, IMapper mapper, ITokenService tokenService, IEmailService emailService)
    {
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    public async Task<TokenPairDto> RegisterUser(RegisterUserDto registerUserDto, HttpRequest httpRequest, IUrlHelper urlHelper)
    {
        CheckRegisterValidness(registerUserDto);
        var newUser = _mapper.Map<AppUser>(registerUserDto);
        var result = await _userManager.CreateAsync(newUser, registerUserDto.password);

        if (!result.Succeeded)
        {
            var errorsStrings = result.Errors.Select(e => e.Description.ToString()).ToList();
            throw new AuthErrorsException(errorsStrings);
        }

        var emailDto = new SendEmailDto()
        {
            email = newUser.Email,
            subject = "Confirm email",
            message = "Для подтверждения почты перейдите по ссылке: "
        };
        await _emailService.SendConfirmationEmail(httpRequest, urlHelper, emailDto);

        var findUser = await _context
                           .Users
                           .FirstOrDefaultAsync(user => user.Email == registerUserDto.email) ??
                       throw new NotFoundException($"User with email {registerUserDto.email} not found");

        await _userManager.AddToRoleAsync(findUser, UserRole.Customer.ToString());
        await _context.SaveChangesAsync();

        var loginCred = new LoginUserDto()
        {
            email = registerUserDto.email,
            password = registerUserDto.password
        };
        return await LoginUser(loginCred);
    }

    public async Task<TokenPairDto> LoginUser(LoginUserDto loginUserDto)
    {
        var findUser = await _userManager
            .FindByEmailAsync(loginUserDto.email) ?? throw new BadRequestException("Invalid credentials");

        var isPasswordValid = await _userManager.CheckPasswordAsync(findUser, loginUserDto.password);
        if (!isPasswordValid)
        {
            throw new BadRequestException("Invalid credentials");
        }

        var user = await _context
                       .Users
                       .FirstOrDefaultAsync(user => user.Email == loginUserDto.email) ??
                   throw new BadRequestException($"Can't find user with email {loginUserDto.email}");

        var roles = await GetUserRoles(user);

        var tokenUser = _mapper.Map<TokenUserDto>(user);
        var accessToken = _tokenService.CreateToken(tokenUser, roles);
        user.RefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(JwtConfig.RefreshLifetime);
        await _userManager.UpdateAsync(user);
        
        return new TokenPairDto()
        {
            accessToken = accessToken,
            refreshToken = user.RefreshToken
        };
    }

    public async Task<TokenPairDto> RefreshToken(TokenPairDto tokenPairDto)
    {
        var info = _tokenService.GetExpiredTokenInfo(tokenPairDto.accessToken);
        if (info == null)
        {
            throw new BadRequestException("Invalid access or refresh token");
        }
        
        var email = info.FindFirst(ClaimTypes.Email)?.Value;
        var userEntity = await _userManager.FindByEmailAsync(email);
        
        if (userEntity == null || 
            userEntity.RefreshToken != tokenPairDto.refreshToken ||
            userEntity.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            throw new BadRequestException("Invalid access or refresh token");
        }

        var tokenUser = _mapper.Map<TokenUserDto>(userEntity);
        var roles = await GetUserRoles(userEntity);
        
        var newAccess = _tokenService.CreateToken(tokenUser, roles);
        var newRefresh = _tokenService.GenerateRefreshToken();

        userEntity.RefreshToken = newRefresh;
        await _userManager.UpdateAsync(userEntity);
        
        
        return new TokenPairDto()
        {
            accessToken = newAccess,
            refreshToken = newRefresh
        };
    }

    public async Task LogoutUser(string email)
    {
        var user = await _userManager.FindByEmailAsync(email) ?? throw new NotAuthorizedException("Something went wrong");

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        
        await _userManager.UpdateAsync(user);
    }


    
    
    private async Task<List<IdentityRole>> GetUserRoles(AppUser user)
    {
        var rolesIds = await _context
            .UserRoles
            .Where(role => role.UserId == user.Id)
            .Select(role => role.RoleId)
            .ToListAsync();
        var roles = await _context
            .Roles
            .Where(role => rolesIds.Contains(role.Id))
            .ToListAsync();

        return roles;
    }

    private void CheckRegisterValidness(RegisterUserDto rud)
    {
        if (DateTime.UtcNow.Year - rud.birthDate.Year  is < 7 or > 80)
        {
            throw new BadRequestException("You must be in range 7 to 80 years old");
        }
    }
}