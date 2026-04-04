using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace RandevuAPI.Hubs;

[Authorize]
public class GorusmeHub : Hub
{
    // connectionId → kullaniciId eşlemesi
    private static readonly ConcurrentDictionary<string, string> _bagliKullanicilar = new();

    // Kullanıcı bağlandığında kendi id'siyle kayıt olsun
    public async Task KullaniciyiKaydet(string kullaniciId)
    {
        _bagliKullanicilar[Context.ConnectionId] = kullaniciId;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{kullaniciId}");
    }

    // Odaya katıl — randevuId oda adı
    public async Task OdayaKatil(string randevuId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"randevu_{randevuId}");
        await Clients.OthersInGroup($"randevu_{randevuId}")
            .SendAsync("KullaniciBaglandi", Context.ConnectionId);
    }

    // Doktor hazır — hastaya bildirim gönder
    public async Task DoktorHazir(string randevuId, string zoomLink)
    {
        await Clients.OthersInGroup($"randevu_{randevuId}")
            .SendAsync("DoktorHazir", new { RandevuId = randevuId, ZoomLink = zoomLink });
    }

    // Mesaj gönder
    public async Task MesajGonder(string randevuId, string mesaj)
    {
        await Clients.OthersInGroup($"randevu_{randevuId}")
            .SendAsync("MesajAlindi", new
            {
                GonderenId = Context.ConnectionId,
                Mesaj = mesaj,
                Zaman = DateTime.UtcNow
            });
    }

    // Bağlantı kesilince temizle
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_bagliKullanicilar.TryRemove(Context.ConnectionId, out var kullaniciId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{kullaniciId}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}