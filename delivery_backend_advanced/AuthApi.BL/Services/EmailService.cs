using System.Net.Mail;
using AuthApi.Common.Dtos;
using AuthApi.Common.Interfaces;
using AuthApi.DAL.Entities;
using Common.Configurations;
using delivery_backend_advanced.Exceptions;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MimeKit.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace AuthApi.BL.Services;

public class EmailService : IEmailService
{
    private readonly UserManager<AppUser> _userManager;

    public EmailService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task SendEmailAsync(SendEmailDto emailDto)
    {
        var emailMessage = new MimeMessage();
 
        emailMessage.From.Add(new MailboxAddress("Администрация сайта", "admin@metanit.com"));
        emailMessage.To.Add(new MailboxAddress("", emailDto.email));
        emailMessage.Subject = emailDto.subject;
        emailMessage.Body = new TextPart(TextFormat.Html) { Text = emailDto.message };

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(EmailConfig.CorpEmail, EmailConfig.CorpPassword);
        await client.SendAsync(emailMessage);
 
        await client.DisconnectAsync(true);
    }
    
    public async Task ConfirmEmail(Guid userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()) ??
                   throw new CantFindByIdException("user", userId);
        
        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (!result.Succeeded)
        {
            throw new Exception("Error");
        }
    }
}