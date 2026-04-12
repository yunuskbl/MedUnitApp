using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MedUnit.Application.Dtos;
using MedUnit.Application.Interfaces;
using RandevuAPI.Hubs;
using Microsoft.EntityFrameworkCore;
using MedUnit.Infrastructure.Data;

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
    [HttpGet("musait-gunler")]
    [AllowAnonymous]
    public async Task<IActionResult> MusaitGunler([FromQuery] int doktorId)
    {
        var db = HttpContext.RequestServices
            .GetRequiredService<AppDbContext>();

        var bugun = DateTime.UtcNow.Date;
        var sonuc = new List<string>();

        // Tüm randevuları çek, in-memory filtrele
        var tumRandevular = await db.Randevular
            .Where(r => r.DoktorId == doktorId && r.Durum != "iptal")
            .Select(r => r.BaslangicTarihi)
            .ToListAsync();

        for (int i = 1; i <= 15; i++)
        {
            var gun = bugun.AddDays(i);

            if (gun.DayOfWeek == DayOfWeek.Saturday ||
                gun.DayOfWeek == DayOfWeek.Sunday)
                continue;

            var randevuSayisi = tumRandevular
                .Count(r => r.ToUniversalTime().Date == gun);

            if (randevuSayisi < 8)
                sonuc.Add(gun.ToString("yyyy-MM-dd"));
        }

        return Ok(sonuc);
    }

    [HttpGet("musait-saatler")]
    [AllowAnonymous]
    public async Task<IActionResult> MusaitSaatler(
    [FromQuery] int doktorId,
    [FromQuery] string tarih)
    {
        var db = HttpContext.RequestServices
            .GetRequiredService<AppDbContext>();

        var gun = DateTime.SpecifyKind(DateTime.Parse(tarih), DateTimeKind.Utc).Date;

        // Dolu saatler
        var tumRandevular = await db.Randevular
            .Where(r => r.DoktorId == doktorId && r.Durum != "iptal")
            .Select(r => r.BaslangicTarihi)
            .ToListAsync();

        // In-memory filtrele — PostgreSQL Date karşılaştırma sorununu aşar
        var doluSaatler = tumRandevular
            .Where(r => r.ToUniversalTime().Date == gun)
            .Select(r => r.ToUniversalTime().Hour)
            .ToList();

        var tumSaatler = Enumerable.Range(9, 9).ToList();
        var musaitSaatler = tumSaatler
            .Where(s => !doluSaatler.Contains(s))
            .Select(s => $"{s:D2}:00")
            .ToList();

        return Ok(musaitSaatler);
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

}