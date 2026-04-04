using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedUnit.Application.Interfaces;

namespace RandevuAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly IAppDbContext _context;

    public AdminController(IAppDbContext context)
    {
        _context = context;
    }

    // İstatistikler
    [HttpGet("istatistikler")]
    public async Task<IActionResult> Istatistikler()
    {
        var toplamKullanici = await _context.Kullanicilar.CountAsync();
        var toplamHasta = await _context.Kullanicilar.CountAsync(k => k.Rol.ToLower() == "hasta");
        var toplamDoktor = await _context.Kullanicilar.CountAsync(k => k.Rol.ToLower() == "doktor");
        var toplamRandevu = await _context.Randevular.CountAsync();
        var bekleyenRandevu = await _context.Randevular.CountAsync(r => r.Durum == "beklemede");
        var onayliRandevu = await _context.Randevular.CountAsync(r => r.Durum == "onaylandi");
        var iptalRandevu = await _context.Randevular.CountAsync(r => r.Durum == "iptal");

        return Ok(new
        {
            toplamKullanici,
            toplamHasta,
            toplamDoktor,
            toplamRandevu,
            bekleyenRandevu,
            onayliRandevu,
            iptalRandevu
        });
    }

    // Tüm kullanıcılar
    [HttpGet("kullanicilar")]
    public async Task<IActionResult> Kullanicilar()
    {
        var kullanicilar = await _context.Kullanicilar
            .OrderByDescending(k => k.OlusturulmaTarihi)
            .Select(k => new
            {
                k.Id,
                k.Ad,
                k.Soyad,
                k.Email,
                k.Rol,
                k.Aktif,
                k.OlusturulmaTarihi
            }).ToListAsync();

        return Ok(kullanicilar);
    }

    // Kullanıcı aktif/pasif yap
    [HttpPut("kullanici/{id}/aktif")]
    public async Task<IActionResult> AktifDegistir(int id)
    {
        var kullanici = await _context.Kullanicilar.FindAsync(id);
        if (kullanici == null) return NotFound();

        kullanici.Aktif = !kullanici.Aktif;
        await _context.SaveChangesAsync();

        return Ok(new { kullanici.Id, kullanici.Aktif });
    }

    // Kullanıcı rolünü değiştir
    [HttpPut("kullanici/{id}/rol")]
    public async Task<IActionResult> RolDegistir(int id, [FromBody] string yeniRol)
    {
        var kullanici = await _context.Kullanicilar.FindAsync(id);
        if (kullanici == null) return NotFound();

        if (!new[] { "hasta", "doktor", "admin" }.Contains(yeniRol.ToLower()))
            return BadRequest(new { message = "Geçersiz rol." });

        kullanici.Rol = yeniRol.ToLower();
        await _context.SaveChangesAsync();

        return Ok(new { kullanici.Id, kullanici.Rol });
    }

    // Tüm randevular
    [HttpGet("randevular")]
    public async Task<IActionResult> Randevular()
    {
        var randevular = await _context.Randevular
            .Include(r => r.Hasta)
            .Include(r => r.Doktor)
            .OrderByDescending(r => r.BaslangicTarihi)
            .Select(r => new
            {
                r.Id,
                HastaAd = r.Hasta.Ad + " " + r.Hasta.Soyad,
                DoktorAd = r.Doktor.Ad + " " + r.Doktor.Soyad,
                r.BaslangicTarihi,
                r.BitisTarihi,
                r.Durum,
                r.Notlar,
                r.ZoomJoinUrl,
                r.ZoomHostUrl
            }).ToListAsync();

        return Ok(randevular);
    }

    // Randevu sil
    [HttpDelete("randevu/{id}")]
    public async Task<IActionResult> RandevuSil(int id)
    {
        var randevu = await _context.Randevular.FindAsync(id);
        if (randevu == null) return NotFound();

        _context.Randevular.Remove(randevu);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}