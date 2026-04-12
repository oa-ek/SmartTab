using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SmartTab.UI.Models;

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

    public async Task SendOrderReceiptEmailAsync(string toEmail, string userName, int orderId, DateTime orderDate, decimal totalPrice, List<ReceiptItem> items)
    {
        var itemsHtml = string.Join("", items.Select(i =>
        {
            var serialsText = i.SerialNumbers.Any()
                ? string.Join("<br/>", i.SerialNumbers)
                : "—";
            return $@"
                <tr>
                    <td style='padding: 10px 12px; border-bottom: 1px solid #e5e7eb; color: #111827; font-size: 14px;'>{i.ProductName}</td>
                    <td style='padding: 10px 12px; border-bottom: 1px solid #e5e7eb; color: #111827; font-size: 14px; text-align: center;'>{i.Quantity}</td>
                    <td style='padding: 10px 12px; border-bottom: 1px solid #e5e7eb; color: #111827; font-size: 14px; text-align: right;'>{i.UnitPrice:N2} ₴</td>
                    <td style='padding: 10px 12px; border-bottom: 1px solid #e5e7eb; color: #6f00ff; font-size: 13px; font-family: monospace;'>{serialsText}</td>
                </tr>";
        }));

        var body = $@"
            <div style='font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, sans-serif; max-width: 600px; margin: 0 auto; padding: 30px;'>
                <div style='text-align: center; margin-bottom: 30px;'>
                    <h1 style='color: #6f00ff; font-size: 28px; margin: 0;'>SmartTab</h1>
                </div>
                <div style='background: #ffffff; border: 1px solid #e5e7eb; border-radius: 12px; padding: 30px;'>
                    <h2 style='color: #111827; font-size: 20px; margin: 0 0 5px;'>Чек замовлення #{orderId}</h2>
                    <p style='color: #6b7280; font-size: 14px; margin: 0 0 20px;'>{orderDate:dd.MM.yyyy HH:mm}</p>
                    <p style='color: #374151; font-size: 14px; margin: 0 0 20px;'>Привіт, <strong>{userName}</strong>! Дякуємо за покупку.</p>
                    <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
                        <tr style='background: #f9fafb;'>
                            <th style='padding: 10px 12px; text-align: left; font-size: 13px; color: #6b7280; border-bottom: 2px solid #e5e7eb;'>Товар</th>
                            <th style='padding: 10px 12px; text-align: center; font-size: 13px; color: #6b7280; border-bottom: 2px solid #e5e7eb;'>К-сть</th>
                            <th style='padding: 10px 12px; text-align: right; font-size: 13px; color: #6b7280; border-bottom: 2px solid #e5e7eb;'>Ціна</th>
                            <th style='padding: 10px 12px; text-align: left; font-size: 13px; color: #6b7280; border-bottom: 2px solid #e5e7eb;'>Серійний номер</th>
                        </tr>
                        {itemsHtml}
                    </table>
                    <div style='text-align: right; padding-top: 15px; border-top: 2px solid #111827;'>
                        <span style='font-size: 18px; font-weight: bold; color: #111827;'>Всього: {totalPrice:N2} ₴</span>
                    </div>
                </div>
                <p style='color: #9ca3af; font-size: 12px; text-align: center; margin-top: 20px;'>
                    Цей лист було згенеровано автоматично. Дякуємо, що обрали SmartTab!
                </p>
            </div>";

        await SendEmailAsync(toEmail, $"SmartTab — Чек замовлення #{orderId}", body);
    }
}
