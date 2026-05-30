using MedUnit.Application.Dtos;
using MedUnit.Application.Interfaces;
using MedUnit.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace RandevuAPI.Controllers;

[ApiController]
[Route("api/klinik")]
public class KlinikController : ControllerBase
{
    private readonly IAppDbContext _context;

    public KlinikController(IAppDbContext context) => _context = context;

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Listele()
    {
        var klinikler = await _context.Klinikler
            .Select(k => new KlinikDto
            {
                Id = k.Id,
                Ad = k.Ad,
                Adres = k.Adres,
                Telefon = k.Telefon,
                Email = k.Email,
                LogoUrl = k.LogoUrl,
                AbonelikTipi = k.AbonelikTipi,
                AbonelikBitisTarihi = k.AbonelikBitisTarihi,
                Aktif = k.Aktif,
                OlusturulmaTarihi = k.OlusturulmaTarihi,
                UyeSayisi = _context.Kullanicilar.Count(u => u.KlinikId == k.Id)
            })
            .ToListAsync();

        return Ok(klinikler);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Olustur([FromBody] KlinikOlusturDto dto)
    {
        var klinik = new Klinik
        {
            Ad = dto.Ad,
            Adres = dto.Adres,
            Telefon = dto.Telefon,
            Email = dto.Email,
            AbonelikTipi = dto.AbonelikTipi
        };

        _context.Klinikler.Add(klinik);
        await _context.SaveChangesAsync();
        return Ok(new { klinik.Id, klinik.Ad });
    }

    [HttpGet("benim")]
    [Authorize(Roles = "klinik_sahibi")]
    public async Task<IActionResult> BenimKliniğim()
    {
        var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);

        if (kullanici?.KlinikId == null)
            return NotFound(new { mesaj = "Bir kliniğe atanmamışsınız." });

        var klinik = await _context.Klinikler.FindAsync(kullanici.KlinikId);
        if (klinik == null) return NotFound();

        return Ok(new KlinikDto
        {
            Id = klinik.Id,
            Ad = klinik.Ad,
            Adres = klinik.Adres,
            Telefon = klinik.Telefon,
            Email = klinik.Email,
            LogoUrl = klinik.LogoUrl,
            AbonelikTipi = klinik.AbonelikTipi,
            AbonelikBitisTarihi = klinik.AbonelikBitisTarihi,
            Aktif = klinik.Aktif,
            OlusturulmaTarihi = klinik.OlusturulmaTarihi,
            UyeSayisi = await _context.Kullanicilar.CountAsync(u => u.KlinikId == klinik.Id)
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,klinik_sahibi")]
    public async Task<IActionResult> Guncelle(int id, [FromBody] KlinikOlusturDto dto)
    {
        var klinik = await _context.Klinikler.FindAsync(id);
        if (klinik == null) return NotFound();

        klinik.Ad = dto.Ad;
        klinik.Adres = dto.Adres;
        klinik.Telefon = dto.Telefon;
        klinik.Email = dto.Email;

        await _context.SaveChangesAsync();
        return Ok();
    }
}
