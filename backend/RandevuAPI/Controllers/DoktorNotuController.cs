using MedUnit.Application.Dtos;
using MedUnit.Application.Interfaces;
using MedUnit.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace RandevuAPI.Controllers;

[ApiController]
[Route("api/doktor-notu")]
[Authorize]
public class DoktorNotuController : ControllerBase
{
    private readonly IAppDbContext _context;

    public DoktorNotuController(IAppDbContext context) => _context = context;

    [HttpPost]
    [Authorize(Roles = "doktor")]
    public async Task<IActionResult> NotuEkle([FromBody] DoktorNotuOlusturDto dto)
    {
        var doktorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var not = new DoktorNotu
        {
            RandevuId = dto.RandevuId,
            DoktorId = doktorId,
            HastaId = dto.HastaId,
            Not = dto.Not,
            Tani = dto.Tani
        };

        _context.DoktorNotlari.Add(not);
        await _context.SaveChangesAsync();
        return Ok(new { not.Id });
    }

    [HttpGet("randevu/{randevuId}")]
    public async Task<IActionResult> RandevuNotu(int randevuId)
    {
        var notlar = await _context.DoktorNotlari
            .Include(n => n.Doktor)
            .Include(n => n.Hasta)
            .Where(n => n.RandevuId == randevuId)
            .Select(n => new DoktorNotuDto
            {
                Id = n.Id,
                RandevuId = n.RandevuId,
                DoktorAdSoyad = n.Doktor.Ad + " " + n.Doktor.Soyad,
                HastaAdSoyad = n.Hasta.Ad + " " + n.Hasta.Soyad,
                Not = n.Not,
                Tani = n.Tani,
                OlusturulmaTarihi = n.OlusturulmaTarihi
            })
            .ToListAsync();

        return Ok(notlar);
    }

    [HttpGet("hasta/{hastaId}")]
    [Authorize(Roles = "doktor,admin")]
    public async Task<IActionResult> HastaNotu(int hastaId)
    {
        var notlar = await _context.DoktorNotlari
            .Include(n => n.Doktor)
            .Include(n => n.Hasta)
            .Where(n => n.HastaId == hastaId)
            .OrderByDescending(n => n.OlusturulmaTarihi)
            .Select(n => new DoktorNotuDto
            {
                Id = n.Id,
                RandevuId = n.RandevuId,
                DoktorAdSoyad = n.Doktor.Ad + " " + n.Doktor.Soyad,
                HastaAdSoyad = n.Hasta.Ad + " " + n.Hasta.Soyad,
                Not = n.Not,
                Tani = n.Tani,
                OlusturulmaTarihi = n.OlusturulmaTarihi
            })
            .ToListAsync();

        return Ok(notlar);
    }

    [HttpGet("benim")]
    [Authorize(Roles = "doktor")]
    public async Task<IActionResult> BenimNotlarim()
    {
        var doktorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var notlar = await _context.DoktorNotlari
            .Include(n => n.Hasta)
            .Where(n => n.DoktorId == doktorId)
            .OrderByDescending(n => n.OlusturulmaTarihi)
            .Select(n => new DoktorNotuDto
            {
                Id = n.Id,
                RandevuId = n.RandevuId,
                DoktorAdSoyad = string.Empty,
                HastaAdSoyad = n.Hasta.Ad + " " + n.Hasta.Soyad,
                Not = n.Not,
                Tani = n.Tani,
                OlusturulmaTarihi = n.OlusturulmaTarihi
            })
            .ToListAsync();

        return Ok(notlar);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "doktor,admin")]
    public async Task<IActionResult> Sil(int id)
    {
        var doktorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;

        var not = await _context.DoktorNotlari.FindAsync(id);
        if (not == null) return NotFound();

        if (rol != "admin" && not.DoktorId != doktorId)
            return Forbid();

        _context.DoktorNotlari.Remove(not);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
