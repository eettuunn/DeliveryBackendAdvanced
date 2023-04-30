using AuthApi.Common.ConfigClasses;
using AuthApi.Common.Dtos;
using AuthApi.Common.Interfaces;
using AuthApi.DAL;
using AuthApi.DAL.Entities;
using AutoMapper;
using Common.Configurations;
using delivery_backend_advanced.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthApi.BL.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly AuthDbContext _context;
    private readonly IConfiguration _configuration;

    public ProfileService(UserManager<AppUser> userManager, IEmailService emailService, IMapper mapper, ITokenService tokenService, AuthDbContext context, IConfiguration configuration)
    {
        _userManager = userManager;
        _emailService = emailService;
        _mapper = mapper;
        _tokenService = tokenService;
        _context = context;
        _configuration = configuration;
    }

    public async Task ChangePassword(ChangePasswordDto changePasswordDto, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (!await _userManager.CheckPasswordAsync(user, changePasswordDto.oldPassword))
        {
            throw new BadRequestException("Incorrect oldPassword");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _userManager.ResetPasswordAsync(user, token, changePasswordDto.newPassword);
        await _userManager.UpdateAsync(user);
    }

    public async Task ForgotPassword(ForgotPasswordDto forgotPassword, HttpRequest request, IUrlHelper urlHelper)
    {
        var sendEmail = new SendEmailDto()
        {
            email = forgotPassword.email,
            message = "Чтобы изменить пароль перейдите по ссылке: ",
            subject = "Change password"
        };
        
        await _emailService.SendConfirmationPasswordEmail(request, urlHelper, forgotPassword.password, sendEmail);
    }

    public async Task ChangeForgotPassword(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email) ??
                   throw new NotFoundException($"Cant find user with email {email}");
        
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _userManager.ResetPasswordAsync(user, token, password);
        await _userManager.UpdateAsync(user);
    }

    public async Task<ProfileDto> GetProfile(string email)
    {
        var user = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException($"Cant find user with email {email}");
        var profile = _mapper.Map<ProfileDto>(user);
        
        return profile;
    }

    public async Task<TokenPairDto> EditProfile(EditProfileDto editProfileDto, string email, IUrlHelper? url, HttpRequest? request)
    {
        CheckEditValidness(editProfileDto);
        
        var user = await _userManager.FindByEmailAsync(email) ??
                   throw new NotFoundException($"Cant find user with email {email}");

        if (editProfileDto.email != null)
        {
            user.Email = editProfileDto.email;
            user.EmailConfirmed = false;

            var sendEmail = new SendEmailDto()
            {
                email = editProfileDto.email,
                message = "Чтобы подтвердить свою почту, перейдите по ссылке: ",
                subject = "Confirm email"
            };
            
            await _userManager.UpdateAsync(user);
            await _emailService.SendConfirmationEmail(request, url, sendEmail);
        }

        user.PhoneNumber = editProfileDto.phoneNumber ?? user.PhoneNumber;
        user.BirthDate = editProfileDto.birthDate ?? user.BirthDate;
        user.Gender = editProfileDto.gender ?? user.Gender;
        user.UserName = editProfileDto.userName ?? user.UserName;

        var customer = await _context
            .Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.User == user) ?? throw new BadRequestException("Something went wrong");
        customer.Address = editProfileDto.address ?? customer.Address;
        await _context.SaveChangesAsync();
        
        return await GetGeneratedTokenPair(user);
    }




    private async Task<TokenPairDto> GetGeneratedTokenPair(AppUser user)
    {
        var roles = await GetUserRoles(user);
        var tokenUser = _mapper.Map<TokenUserDto>(user);
        var accessToken = _tokenService.CreateToken(tokenUser, roles);
        user.RefreshToken = _tokenService.GenerateRefreshToken();
        
        var jwtConfig = _configuration.GetSection("JwtConfig").Get<JwtConfig>();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwtConfig.RefreshDaysLifeTime);
        
        await _userManager.UpdateAsync(user);
        
        return new TokenPairDto()
        {
            accessToken = accessToken,
            refreshToken = user.RefreshToken
        };
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

    private void CheckEditValidness(EditProfileDto epd)
    {
        if (epd.birthDate != null && (DateTime.UtcNow.Year - epd.birthDate.Value.Year is < 7 or > 80))
        {
            throw new BadRequestException("You must be in range 7 to 80 years old");
        }
    }
}