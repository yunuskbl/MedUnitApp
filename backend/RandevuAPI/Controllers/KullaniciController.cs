using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedUnit.Application.Interfaces;
using MedUnit.Application.Dtos;

namespace RandevuAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KullaniciController : ControllerBase
{
    private readonly IAppDbContext _context;
    private readonly IKullaniciService _kullaniciService;

    public KullaniciController(IAppDbContext context, IKullaniciService kullaniciService)
    {
        _context = context;
        _kullaniciService = kullaniciService;
    }

    [HttpGet("kullanicilar")]
    public async Task<IActionResult> Kullanicilar()
    {
        var kullanicilar = await _context.Kullanicilar
            .Select(k => new
            {
                k.Id,
                k.Ad,
                k.Soyad,
                k.Email,
                k.Rol,
                k.Uzmanlik,
                k.Aktif
            })
            .ToListAsync();

        return Ok(kullanicilar);
    }

    [HttpGet("profil")]
    [Authorize]
    public async Task<IActionResult> Profil()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var kullanici = await _context.Kullanicilar.FindAsync(id);
        if (kullanici == null) return NotFound();

        return Ok(new
        {
            kullanici.Id,
            kullanici.Ad,
            kullanici.Soyad,
            kullanici.Email,
            kullanici.Rol,
            kullanici.Uzmanlik,
            kullanici.Telefon,
            kullanici.KlinikId
        });
    }

    [HttpPut("profil")]
    [Authorize]
    public async Task<IActionResult> ProfilGuncelle([FromBody] ProfilGuncelleDto dto)
    {
        try
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _kullaniciService.ProfilGuncelleAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("sifre-degistir")]
    [Authorize]
    public async Task<IActionResult> SifreDegistir([FromBody] SifreDegistirDto dto)
    {
        try
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _kullaniciService.SifreDegistirAsync(id, dto);
            return Ok(new { message = "Şifreniz başarıyla güncellendi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
