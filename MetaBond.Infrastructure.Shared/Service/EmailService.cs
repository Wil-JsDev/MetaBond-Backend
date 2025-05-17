using MailKit.Security;
using MetaBond.Application.DTOs.Email;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Domain.Settings;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MetaBond.Infrastructure.Shared.Service;

public sealed class EmailService(IOptions<MailSettings> mailSettings) : IEmailService
{
    private MailSettings _mailSettings { get; } = mailSettings.Value;

    public async Task SendEmailAsync(EmailRequestDTo request)
    {
        try
        {
            MimeMessage email = new();
            email.Sender = MailboxAddress.Parse (_mailSettings.EmailFrom);
            email.To.Add(MailboxAddress.Parse(request.To)); 
            email.Subject = request.Subject;
            BodyBuilder builder = new()
            {
                HtmlBody = request.Body
            };
            email.Body = builder.ToMessageBody();

            //Configuration SMTP
            using MailKit.Net.Smtp.SmtpClient smtp = new();
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await smtp.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            // ignored
        }
    }
    
}