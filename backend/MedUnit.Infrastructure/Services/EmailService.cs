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
        var apiKey = _config["Resend:ApiKey"];

        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var payload = new
        {
            from = "MedUnit <onboarding@resend.dev>",
            to = new[] { alici },
            subject = konu,
            html = icerik
        };

        var response = await http.PostAsJsonAsync("https://api.resend.com/emails", payload);
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"==> RESEND RESPONSE: {response.StatusCode} - {body}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Mail gönderilemedi: {body}");
    }
}