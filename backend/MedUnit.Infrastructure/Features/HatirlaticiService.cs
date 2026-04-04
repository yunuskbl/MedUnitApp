using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MedUnit.Application.Interfaces;

namespace MedUnit.Infrastructure.Features;

    public class HatirlaticiService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<HatirlaticiService> _logger;

        public HatirlaticiService(IServiceScopeFactory scopeFactory, ILogger<HatirlaticiService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hatırlatıcı servisi başladı.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await HatirlaticlariKontrolEtAsync();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task HatirlaticlariKontrolEtAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var simdi = DateTime.UtcNow;
            var hedefAralik = simdi.AddHours(23);

            var randevular = await context.Randevular
                .Include(r => r.Hasta)
                .Where(r =>
                    r.Durum == "onaylandi" &&
                    !r.HatirlaticiGonderildi &&
                    r.BaslangicTarihi >= hedefAralik &&
                    r.BaslangicTarihi <= simdi.AddHours(25))
                .ToListAsync();

            foreach (var randevu in randevular)
            {
                try
                {
                    var icerik = EmailSablonu(randevu.Hasta.Ad, randevu.BaslangicTarihi);

                    await emailService.GonderAsync(
                        randevu.Hasta.Email,
                        "Yarınki Randevunuzu Hatırlatırız 📅",
                        icerik);

                    randevu.HatirlaticiGonderildi = true;
                    _logger.LogInformation("Hatırlatıcı gönderildi: {Email}", randevu.Hasta.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Email gönderilemedi: {Email}", randevu.Hasta.Email);
                }
            }

            if (randevular.Any())
                await context.SaveChangesAsync();
        }

        private static string EmailSablonu(string ad, DateTime tarih) => $"""
        <div style="font-family:Arial,sans-serif;max-width:500px;margin:auto">
            <h2 style="color:#4f46e5">MedUnit — Randevu Hatırlatıcı</h2>
            <p>Merhaba <strong>{ad}</strong>,</p>
            <p>Yarın saat <strong>{tarih:HH:mm}</strong> için randevunuz bulunmaktadır.</p>
            <p style="background:#f3f4f6;padding:12px;border-radius:8px">
                📅 Tarih: {tarih:dd MMMM yyyy}<br/>
                🕐 Saat: {tarih:HH:mm}
            </p>
            <p>Lütfen zamanında teşrif ediniz.</p>
            <hr/>
            <small style="color:#9ca3af">Bu email otomatik gönderilmiştir.</small>
        </div>
        """;
    }
