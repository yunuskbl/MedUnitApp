using Microsoft.AspNetCore.Mvc;
using MedUnit.Application.Interfaces;
using MedUnit.Application.Dtos;

namespace RandevuAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IKullaniciService _kullaniciService;

    public AuthController(IKullaniciService kullaniciService)
    {
        _kullaniciService = kullaniciService;
    }

    [HttpPost("kayit")]
    public async Task<IActionResult> Kayit([FromBody] KayitDto dto)
    {
        try
        {
            var result = await _kullaniciService.KayitOlAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("giris")]
    public async Task<IActionResult> Giris([FromBody] GirisDto dto)
    {
        try
        {
            var result = await _kullaniciService.GirisYapAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}