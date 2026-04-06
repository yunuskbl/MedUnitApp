using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MedUnit.Application.Dtos;
using MedUnit.Application.Interfaces;
using RandevuAPI.Hubs;

namespace RandevuAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RandevuController : ControllerBase
{
    private readonly IRandevuService _randevuService;
    private readonly IHubContext<GorusmeHub> _hubContext;
    private readonly IZoomService _zoomService;

    public RandevuController(
        IRandevuService randevuService,
        IHubContext<GorusmeHub> hubContext,
        IZoomService zoomService)
    {
        _randevuService = randevuService;
        _hubContext = hubContext;
        _zoomService = zoomService;
    }

    private int KullaniciId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string Rol => User.FindFirstValue(ClaimTypes.Role)?.ToLower() ?? string.Empty;

    [HttpPost]
    public async Task<IActionResult> Olustur([FromBody] RandevuOlusturDto dto)
    {
        try
        {
            var result = await _randevuService.OlusturAsync(KullaniciId, dto);

            await _hubContext.Clients
                .Group($"user_{dto.DoktorId}")
                .SendAsync("RandevuGuncellendi", new
                {
                    RandevuId = result.Id,
                    Durum = "beklemede",
                    Mesaj = "Yeni bir randevu talebiniz var!"
                });

            return CreatedAtAction(nameof(Listele), result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Listele()
    {
        var result = await _randevuService.ListeleAsync(KullaniciId, Rol);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "doktor,admin")]
    public async Task<IActionResult> Guncelle(int id, [FromBody] RandevuGuncelleDto dto)
    {
        try
        {
            var result = await _randevuService.GuncelleAsync(id, KullaniciId,dto);

            if (dto.Durum == "onaylandi")
            {
                try { await _zoomService.MeetingOlusturAsync(id); }
                catch (Exception zoomEx)
                {
                    Console.WriteLine($"Zoom hatası: {zoomEx.Message}");
                }
            }

            await _hubContext.Clients
                .Group($"user_{result.HastaId}")
                .SendAsync("RandevuGuncellendi", new
                {
                    RandevuId = id,
                    Durum = dto.Durum,
                    Mesaj = dto.Durum == "onaylandi"
                        ? "Randevunuz onaylandı!"
                        : "Randevunuz güncellendi."
                });

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/iptal")]
    public async Task<IActionResult> Iptal(int id)
    {
        try
        {
            await _randevuService.SilAsync(id, KullaniciId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Sil(int id)
    {
        try
        {
            await _randevuService.SilAsync(id, KullaniciId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("musait-saatler")]
    [AllowAnonymous]
    public async Task<IActionResult> MusaitSaatler([FromQuery] int doktorId, [FromQuery] string tarih)
    {
        if (!DateTime.TryParse(tarih, out var secilenTarih))
            return BadRequest(new { message = "Geçersiz tarih." });

        // Tüm çalışma saatleri (09:00 - 17:00, 45'er dakika)
        var tumSaatler = new List<string>
    {
        "09:00", "09:45", "10:30", "11:15",
        "13:00", "13:45", "14:30", "15:15", "16:00"
    };

        // O gün o doktorun dolu randevularını çek
        var mevcutRandevular = await _randevuService.DoktorRandevulariAsync(
            doktorId, secilenTarih);

        var doluSaatler = mevcutRandevular
            .Where(r => r.Durum != "iptal")
            .Select(r => r.BaslangicTarihi.ToString("HH:mm"))
            .ToList();

        var musaitSaatler = tumSaatler
            .Select(s => new
            {
                saat = s,
                musait = !doluSaatler.Contains(s)
            })
            .ToList();

        return Ok(musaitSaatler);
    }
}