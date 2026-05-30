using MedUnit.Application.Dtos;
using MedUnit.Application.Interfaces;
using MedUnit.Domain.Entities;
using MedUnit.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

    public async Task<ProfilDto> GetProfilAsync(string kullaniciId)
    {

        var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Id.ToString() == kullaniciId); ;
        if (kullanici == null) throw new Exception("Kullanıcı bulunamadı.");

        return new ProfilDto
        {
            AdSoyad = kullanici.Ad + " " + kullanici.Soyad,
            Biyografi = kullanici.Biyografi,
            DanismanlikUcreti = kullanici.DanismanlikUcreti,
            UzmanlikAlanlari = kullanici.UzmanlikAlanlari ?? new List<string>(),
            Sertifikalar = kullanici.Sertifikalar?.Select(s => new SertifikaDto
            {
                Baslik = s.Baslik,
                Kurum = s.Kurum,
                Yil = s.Yil,
                Ikon = s.Ikon
            }).ToList() ?? new(),
            CalismaSaatleri = kullanici.CalismaSaatleri?.Select(c => new CalismaSaatiDto
            {
                Gun = c.Gun,
                BaslangicSaati = c.BaslangicSaati,
                BitisSaati = c.BitisSaati,
                Izinli = c.Izinli
            }).ToList() ?? new()
        };
    }

    public async Task UpdateProfilAsync(string kullaniciId, ProfilDto profil)
    {
        var kullanici = await _context.Kullanicilar
        .FirstOrDefaultAsync(k => k.Id.ToString() == kullaniciId);
        if (kullanici == null) throw new Exception("Kullanıcı bulunamadı.");

        kullanici.Biyografi = profil.Biyografi;
        kullanici.DanismanlikUcreti = profil.DanismanlikUcreti;
        kullanici.UzmanlikAlanlari = profil.UzmanlikAlanlari;
        // sertifika ve çalışma saatleri güncelleme mantığı

        _context.Kullanicilar.Update(kullanici);
        await _context.SaveChangesAsync();
    }
}
