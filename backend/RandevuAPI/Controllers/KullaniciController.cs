using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedUnit.Application.Interfaces;

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
    [Authorize(Roles ="admin")]
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

        return Ok(new
        {
            kullanici.Id,
            kullanici.Ad,
            kullanici.Soyad,
            kullanici.Email,
            kullanici.Rol
        });
    }
}