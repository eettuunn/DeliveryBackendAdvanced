using System.Net.Mail;
using AuthApi.Common.Dtos;
using AuthApi.Common.Interfaces;
using Common.Configurations;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace AuthApi.BL.Services;

public class EmailService : IEmailService
{
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
}