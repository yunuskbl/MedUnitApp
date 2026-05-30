using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace MedUnit.Infrastructure.Features;

public class SmsReminderService
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmsReminderService> _logger;

    public SmsReminderService(IConfiguration config, ILogger<SmsReminderService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task HatirlaticiGonderAsync(string telefon, string ad, DateTime tarih)
    {
        var accountSid = _config["Twilio:AccountSid"];
        var authToken  = _config["Twilio:AuthToken"];
        var fromNumber = _config["Twilio:FromNumber"];

        if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromNumber))
        {
            _logger.LogDebug("Twilio yapılandırması eksik — SMS atlanıyor.");
            return;
        }

        TwilioClient.Init(accountSid, authToken);

        var metin = $"MedUnit: Merhaba {ad}, yarın saat {tarih:HH:mm} için randevunuz bulunmaktadır. İyi günler!";

        try
        {
            await MessageResource.CreateAsync(
                body: metin,
                from: new Twilio.Types.PhoneNumber(fromNumber),
                to:   new Twilio.Types.PhoneNumber(telefon));

            _logger.LogInformation("SMS gönderildi: {Telefon}", telefon);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMS gönderilemedi: {Telefon}", telefon);
        }
    }
}
