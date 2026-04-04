using MailKit.Net.Smtp;
using MailKit.Security;
using MedUnit.Application.Interfaces;
using MedUnit.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace MedUnit.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task GonderAsync(string alici, string konu, string icerik)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(
            _config["Email:GonderenAd"],
            _config["Email:KullaniciAdi"]));

        email.To.Add(MailboxAddress.Parse(alici ?? _config["Email:ToAddress"]));

        email.Subject = konu;
        email.Body = new TextPart("html") { Text = icerik };

        using var smtp = new SmtpClient();

        try
        {
            await smtp.ConnectAsync(
                _config["Email:Host"],
                int.Parse(_config["Email:Port"]!),
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _config["Email:KullaniciAdi"],
                _config["Email:Sifre"]);

            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            Console.WriteLine("MAIL ERROR: " + ex.Message);
            throw;
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}