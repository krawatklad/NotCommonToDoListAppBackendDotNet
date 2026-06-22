using Application.Common.Configurations;
using Application.Common.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Common;

public class EmailSender(IOptions<EmailSenderOptions> options) : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var settings = options.Value;
        var mimeMessage = new MimeMessage();
        
        mimeMessage.From.Add(new MailboxAddress(settings.FromName, settings.From));
        mimeMessage.To.Add(MailboxAddress.Parse(email));
        mimeMessage.Subject = subject;
        mimeMessage.Body = new TextPart(TextFormat.Html) { Text = message };
        
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(settings.Host, settings.Port, settings.UseSsl);
        if (!string.IsNullOrEmpty(settings.Username) || !string.IsNullOrEmpty(settings.Password))
        {
            await smtp.AuthenticateAsync(settings.Username, settings.Password);
        }
        
        await smtp.SendAsync(mimeMessage);
        await smtp.DisconnectAsync(true);
    }
}
