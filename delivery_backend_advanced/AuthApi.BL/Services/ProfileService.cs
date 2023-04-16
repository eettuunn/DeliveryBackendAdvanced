using AuthApi.Common.Dtos;
using AuthApi.Common.Interfaces;
using AuthApi.DAL.Entities;
using AutoMapper;
using delivery_backend_advanced.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.BL.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public ProfileService(UserManager<AppUser> userManager, IEmailService emailService, IMapper mapper)
    {
        _userManager = userManager;
        _emailService = emailService;
        _mapper = mapper;
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
}