using Microsoft.AspNetCore.Mvc;
using MedUnit.Application.Interfaces;
using MedUnit.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace RandevuAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IKullaniciService _kullaniciService;
    private readonly IAppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;

    public AuthController(
        IKullaniciService kullaniciService,
        IAppDbContext context,
        IEmailService emailService,
        IConfiguration config)
    {
        _kullaniciService = kullaniciService;
        _context = context;
        _emailService = emailService;
        _config = config;
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

    [HttpPost("unuttum-sifre")]
    public async Task<IActionResult> UnuttumSifre([FromBody] SifreSifirlamaIstekDto dto)
    {
        // Güvenlik için kullanıcı bulunamasa bile Ok döndür
        var kullanici = await _context.Kullanicilar
            .FirstOrDefaultAsync(k => k.Email == dto.Email);

        if (kullanici != null)
        {
            var token = Guid.NewGuid().ToString("N");
            kullanici.SifreSifirlamaToken = token;
            kullanici.SifreSifirlamaTokenSon = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            var frontendUrl = _config["FrontendUrl"] ?? "http://localhost:4200";
            var resetLink = $"{frontendUrl}/sifre-yenile?token={token}";

            var icerik = $"""
                <div style="font-family:Arial,sans-serif;max-width:500px;margin:auto;padding:24px;border:1px solid #e5e7eb;border-radius:12px">
                  <h2 style="color:#4f46e5;margin-bottom:8px">MedUnit — Şifre Sıfırlama</h2>
                  <p style="color:#374151">Merhaba <strong>{kullanici.Ad}</strong>,</p>
                  <p style="color:#374151">Şifre sıfırlama talebinizi aldık. Aşağıdaki butona tıklayarak yeni şifrenizi belirleyebilirsiniz.</p>
                  <div style="text-align:center;margin:28px 0">
                    <a href="{resetLink}" style="background:#4f46e5;color:white;padding:14px 28px;border-radius:8px;text-decoration:none;font-weight:600;font-size:15px">Şifremi Sıfırla</a>
                  </div>
                  <p style="color:#9ca3af;font-size:12px">Bu link <strong>1 saat</strong> geçerlidir. Eğer bu talebi siz yapmadıysanız bu emaili görmezden gelebilirsiniz.</p>
                </div>
                """;

            await _emailService.GonderAsync(kullanici.Email, "MedUnit — Şifre Sıfırlama", icerik);
        }

        return Ok(new { message = "Eğer bu email kayıtlıysa, sıfırlama linki gönderildi." });
    }

    [HttpPost("sifre-yenile")]
    public async Task<IActionResult> SifreYenile([FromBody] SifreYenilemeDto dto)
    {
        var kullanici = await _context.Kullanicilar
            .FirstOrDefaultAsync(k => k.SifreSifirlamaToken == dto.Token);

        if (kullanici == null || kullanici.SifreSifirlamaTokenSon < DateTime.UtcNow)
            return BadRequest(new { message = "Geçersiz veya süresi dolmuş link." });

        kullanici.SifreHash = BCrypt.Net.BCrypt.HashPassword(dto.YeniSifre);
        kullanici.SifreSifirlamaToken = null;
        kullanici.SifreSifirlamaTokenSon = null;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Şifreniz başarıyla güncellendi." });
    }
}
