using MedUnit.Application.Dtos;
using MedUnit.Application.Interfaces;
using MedUnit.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace RandevuAPI.Controllers;

[ApiController]
[Route("api/tibbiDosya")]
[Authorize]
public class TibbiDosyaController : ControllerBase
{
    private readonly IAppDbContext _context;

    public TibbiDosyaController(IAppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> DosyaYukle(IFormFile dosya)
    {
        if (dosya == null || dosya.Length == 0)
            return BadRequest(new { mesaj = "Dosya seçilmedi." });

        if (dosya.Length > 10 * 1024 * 1024)
            return BadRequest(new { mesaj = "Dosya boyutu 10 MB'yi aşamaz." });

        var izinliTipler = new[] { "application/pdf", "image/jpeg", "image/png", "image/jpg" };
        if (!izinliTipler.Contains(dosya.ContentType))
            return BadRequest(new { mesaj = "Sadece PDF ve görsel dosyaları (JPG, PNG) yüklenebilir." });

        var hastaId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        using var ms = new MemoryStream();
        await dosya.CopyToAsync(ms);

        var tibbiDosya = new TibbiDosya
        {
            HastaId = hastaId,
            DosyaAdi = dosya.FileName,
            DosyaTipi = dosya.ContentType,
            DosyaVerisi = ms.ToArray(),
            DosyaBoyutu = dosya.Length
        };

        _context.TibbiDosyalar.Add(tibbiDosya);
        await _context.SaveChangesAsync();

        return Ok(new { tibbiDosya.Id, tibbiDosya.DosyaAdi });
    }

    [HttpGet]
    public async Task<IActionResult> Listele([FromQuery] int? hastaId)
    {
        var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;

        IQueryable<TibbiDosya> query = _context.TibbiDosyalar;

        if (rol == "doktor" || rol == "admin")
        {
            if (hastaId.HasValue)
                query = query.Where(d => d.HastaId == hastaId.Value);
        }
        else
        {
            query = query.Where(d => d.HastaId == kullaniciId);
        }

        var dosyalar = await query
            .OrderByDescending(d => d.YuklemeTarihi)
            .Select(d => new TibbiDosyaDto
            {
                Id = d.Id,
                HastaId = d.HastaId,
                DosyaAdi = d.DosyaAdi,
                DosyaTipi = d.DosyaTipi,
                DosyaBoyutu = d.DosyaBoyutu,
                YuklemeTarihi = d.YuklemeTarihi
            })
            .ToListAsync();

        return Ok(dosyalar);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> DosyaIndir(int id)
    {
        var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;

        var dosya = await _context.TibbiDosyalar.FindAsync(id);
        if (dosya == null) return NotFound();

        if (rol != "doktor" && rol != "admin" && dosya.HastaId != kullaniciId)
            return Forbid();

        return File(dosya.DosyaVerisi, dosya.DosyaTipi, dosya.DosyaAdi);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Sil(int id)
    {
        var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;

        var dosya = await _context.TibbiDosyalar.FindAsync(id);
        if (dosya == null) return NotFound();

        if (rol != "admin" && dosya.HastaId != kullaniciId)
            return Forbid();

        _context.TibbiDosyalar.Remove(dosya);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
