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
        Console.WriteLine($"==> SMTP BASLIYOR");
        Console.WriteLine($"==> Host: {_config["Email:Host"]}");
        Console.WriteLine($"==> Port: {_config["Email:Port"]}");
        Console.WriteLine($"==> KullaniciAdi: {_config["Email:KullaniciAdi"]}");
        Console.WriteLine($"==> Alici: {alici}");

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
            Console.WriteLine("==> ConnectAsync deneniyor...");
            await smtp.ConnectAsync(
                _config["Email:Host"],
                int.Parse(_config["Email:Port"]!),
                SecureSocketOptions.StartTls);
            Console.WriteLine("==> Connected!");

            await smtp.AuthenticateAsync(
                _config["Email:KullaniciAdi"],
                _config["Email:Sifre"]);
            Console.WriteLine("==> Auth OK!");

            await smtp.SendAsync(email);
            Console.WriteLine("==> MAIL GONDERILDI!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"==> SMTP HATA TURU: {ex.GetType().Name}");
            Console.WriteLine($"==> SMTP HATA MESAJI: {ex.Message}");
            throw;
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}