using System.Net.Http;
using System.Text;
using System.Text.Json;
using MedUnit.Application.Interfaces;
using Microsoft.Extensions.Configuration;

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

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await http.PostAsync("https://api.resend.com/emails", content);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Mail gönderilemedi: {body}");
    }
}