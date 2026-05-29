using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MedUnit.Application.Dtos;
using MedUnit.Application.Interfaces;
using MedUnit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MedUnit.Application.Services;

public class KullaniciService : IKullaniciService
{
    private readonly IAppDbContext _context;
    private readonly IConfiguration _config;

    public KullaniciService(IAppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponseDto> KayitOlAsync(KayitDto dto)
    {
        var mevcutKullanici = await _context.Kullanicilar
            .FirstOrDefaultAsync(k => k.Email == dto.Email);

        if (mevcutKullanici != null)
            throw new Exception("Bu email adresi zaten kayıtlı.");

        var kullanici = new Kullanici
        {
            Ad = dto.Ad,
            Soyad = dto.Soyad,
            Email = dto.Email,
            SifreHash = BCrypt.Net.BCrypt.HashPassword(dto.Sifre),
            Rol = dto.Rol
        };

        _context.Kullanicilar.Add(kullanici);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Id = kullanici.Id,
            Token = TokenOlustur(kullanici),
            Ad = kullanici.Ad,
            Soyad = kullanici.Soyad,
            Email = kullanici.Email,
            Rol = kullanici.Rol
        };
    }

    public async Task<AuthResponseDto> GirisYapAsync(GirisDto dto)
    {
        var kullanici = await _context.Kullanicilar
            .FirstOrDefaultAsync(k => k.Email == dto.Email);

        if (kullanici == null || !BCrypt.Net.BCrypt.Verify(dto.Sifre, kullanici.SifreHash))
            throw new Exception("Email veya şifre hatalı.");

        return new AuthResponseDto
        {
            Id = kullanici.Id,
            Token = TokenOlustur(kullanici),
            Ad = kullanici.Ad,
            Soyad = kullanici.Soyad,
            Email = kullanici.Email,
            Rol = kullanici.Rol,
            Uzmanlik = kullanici.Uzmanlik
        };
    }

    public async Task<AuthResponseDto> ProfilGuncelleAsync(int kullaniciId, ProfilGuncelleDto dto)
    {
        var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId)
            ?? throw new Exception("Kullanıcı bulunamadı.");

        if (!string.IsNullOrWhiteSpace(dto.Ad)) kullanici.Ad = dto.Ad;
        if (!string.IsNullOrWhiteSpace(dto.Soyad)) kullanici.Soyad = dto.Soyad;
        if (dto.Uzmanlik != null) kullanici.Uzmanlik = dto.Uzmanlik;
        if (dto.Telefon != null) kullanici.Telefon = dto.Telefon;

        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Id = kullanici.Id,
            Token = TokenOlustur(kullanici),
            Ad = kullanici.Ad,
            Soyad = kullanici.Soyad,
            Email = kullanici.Email,
            Rol = kullanici.Rol,
            Uzmanlik = kullanici.Uzmanlik,
            Telefon = kullanici.Telefon,
            KlinikId = kullanici.KlinikId
        };
    }

    public async Task SifreDegistirAsync(int kullaniciId, SifreDegistirDto dto)
    {
        var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId)
            ?? throw new Exception("Kullanıcı bulunamadı.");

        if (!BCrypt.Net.BCrypt.Verify(dto.MevcutSifre, kullanici.SifreHash))
            throw new Exception("Mevcut şifre hatalı.");

        kullanici.SifreHash = BCrypt.Net.BCrypt.HashPassword(dto.YeniSifre);
        await _context.SaveChangesAsync();
    }

    private string TokenOlustur(Kullanici kullanici)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
            new Claim(ClaimTypes.Email, kullanici.Email),
            new Claim(ClaimTypes.Role, kullanici.Rol),
            new Claim("role", kullanici.Rol)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
