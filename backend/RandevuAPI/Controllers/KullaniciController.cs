using MedUnit.Application.Dtos;
using MedUnit.Application.Interfaces;
using MedUnit.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace RandevuAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KullaniciController : ControllerBase
{
    private readonly IAppDbContext _context;

    public KullaniciController(IAppDbContext context)
    {
        _context = context;
    }

    [HttpGet("kullanicilar")]
    public async Task<IActionResult> Kullanicilar()
    {
        var kullanicilar = await _context.Kullanicilar.ToListAsync();
        return Ok(kullanicilar);
    }

    [HttpGet("profil")]
    [Authorize]
    public async Task<IActionResult> Profil()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var kullanici = await _context.Kullanicilar.FindAsync(id);
        if (kullanici == null) return NotFound();

        return Ok(new ProfilDto
        {
            AdSoyad = $"{kullanici.Ad} {kullanici.Soyad}",
            Biyografi = kullanici.Biyografi,
            DanismanlikUcreti = kullanici.DanismanlikUcreti,
            UzmanlikAlanlari = kullanici.UzmanlikAlanlari,
            Sertifikalar = kullanici.Sertifikalar.Select(s => new SertifikaDto
            {
                Baslik = s.Baslik,
                Kurum = s.Kurum,
                Yil = s.Yil,
                Ikon = s.Ikon
            }).ToList(),
            CalismaSaatleri = kullanici.CalismaSaatleri.Select(c => new CalismaSaatiDto
            {
                Gun = c.Gun,
                BaslangicSaati = c.BaslangicSaati,
                BitisSaati = c.BitisSaati,
                Izinli = c.Izinli
            }).ToList()
        });
    }

    [HttpPut("profil")]
    [Authorize]
    public async Task<IActionResult> UpdateProfil([FromBody] ProfilDto dto)
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var kullanici = await _context.Kullanicilar.FindAsync(id);
        if (kullanici == null) return NotFound();

        kullanici.Biyografi = dto.Biyografi;
        kullanici.DanismanlikUcreti = dto.DanismanlikUcreti;
        kullanici.UzmanlikAlanlari = dto.UzmanlikAlanlari;
        kullanici.Sertifikalar = dto.Sertifikalar.Select(s => new Sertifika
        {
            Baslik = s.Baslik,
            Kurum = s.Kurum,
            Yil = s.Yil,
            Ikon = s.Ikon
        }).ToList();
        kullanici.CalismaSaatleri = dto.CalismaSaatleri.Select(c => new CalismaSaati
        {
            Gun = c.Gun,
            BaslangicSaati = c.BaslangicSaati,
            BitisSaati = c.BitisSaati,
            Izinli = c.Izinli
        }).ToList();

        await _context.SaveChangesAsync(CancellationToken.None);
        return Ok();
    }
}