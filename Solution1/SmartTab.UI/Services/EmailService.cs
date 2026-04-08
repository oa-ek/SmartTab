using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace SmartTab.UI.Services;

public class EmailService
{
    private readonly SmtpSettings _smtp;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtp = smtpSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_smtp.Email, _smtp.DisplayName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(_smtp.Host, _smtp.Port)
        {
            Credentials = new NetworkCredential(_smtp.Email, _smtp.Password),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        var subject = "SmartTab — Відновлення паролю";
        var body = $@"
            <div style='font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, sans-serif; max-width: 500px; margin: 0 auto; padding: 30px;'>
                <div style='text-align: center; margin-bottom: 30px;'>
                    <h1 style='color: #6f00ff; font-size: 28px; margin: 0;'>SmartTab</h1>
                </div>
                <div style='background: #ffffff; border: 1px solid #e5e7eb; border-radius: 12px; padding: 30px;'>
                    <h2 style='color: #111827; font-size: 20px; margin: 0 0 15px;'>Відновлення паролю</h2>
                    <p style='color: #6b7280; font-size: 14px; line-height: 1.6; margin: 0 0 20px;'>
                        Ви отримали цей лист, тому що було зроблено запит на відновлення паролю для вашого акаунту SmartTab.
                    </p>
                    <div style='text-align: center; margin: 25px 0;'>
                        <a href='{resetLink}' style='display: inline-block; background-color: #6f00ff; color: #ffffff; text-decoration: none; padding: 12px 32px; border-radius: 8px; font-weight: bold; font-size: 14px;'>
                            Скинути пароль
                        </a>
                    </div>
                    <p style='color: #9ca3af; font-size: 12px; line-height: 1.5; margin: 20px 0 0;'>
                        Посилання дійсне протягом 1 години. Якщо ви не робили цей запит — просто ігноруйте цей лист.
                    </p>
                </div>
            </div>";

        await SendEmailAsync(toEmail, subject, body);
    }
}
