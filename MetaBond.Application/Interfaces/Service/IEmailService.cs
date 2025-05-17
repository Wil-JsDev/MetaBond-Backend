using MetaBond.Application.DTOs.Email;

namespace MetaBond.Application.Interfaces.Service;

public interface IEmailService
{
    Task SendEmailAsync(EmailRequestDTo emailRequest);
}