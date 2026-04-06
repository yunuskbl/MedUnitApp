using MedUnit.Application.Dtos;
using MedUnit.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedUnit.Application.Services;

public class RandevuService : IRandevuService
{
    private readonly IAppDbContext _context;

    public RandevuService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<RandevuResponseDto> OlusturAsync(int hastaId, RandevuOlusturDto dto)
    {
        if (dto.BaslangicTarihi.ToUniversalTime() <= DateTime.UtcNow.AddMinutes(-5))
            throw new Exception("Geçmiş bir tarihe randevu oluşturamazsınız.");

        var sure = (dto.BitisTarihi - dto.BaslangicTarihi).TotalMinutes;
        if (sure < 15)
            throw new Exception("Randevu süresi en az 15 dakika olmalıdır.");

        if (hastaId == dto.DoktorId)
            throw new Exception("Kendinize randevu oluşturamazsınız.");

        var cakisma = await _context.Randevular.AnyAsync(r =>
            r.DoktorId == dto.DoktorId &&
            r.Durum != "iptal" &&
            r.BaslangicTarihi < dto.BitisTarihi &&
            r.BitisTarihi > dto.BaslangicTarihi);

        if (cakisma)
            throw new Exception("Doktorun bu saatte başka randevusu bulunmaktadır.");

        var randevu = new Randevu
        {
            HastaId = hastaId,
            DoktorId = dto.DoktorId,
            BaslangicTarihi = dto.BaslangicTarihi,
            BitisTarihi = dto.BitisTarihi,
            Notlar = dto.Notlar
        };

        _context.Randevular.Add(randevu);
        await _context.SaveChangesAsync();

        return await RandevuDonusturAsync(randevu.Id);
    }

    public async Task<List<RandevuResponseDto>> ListeleAsync(int kullaniciId, string rol)
    {
        var query = _context.Randevular
            .Include(r => r.Hasta)
            .Include(r => r.Doktor)
            .AsQueryable();

        query = rol.ToLower() == "doktor" || rol.ToLower() == "admin"
            ? query.Where(r => r.DoktorId == kullaniciId)
            : query.Where(r => r.HastaId == kullaniciId);

        return await query
            .OrderByDescending(r => r.BaslangicTarihi)
            .Select(r => new RandevuResponseDto
            {
                Id = r.Id,
                HastaId = r.HastaId,
                DoktorId = r.DoktorId,
                HastaAd = r.Hasta.Ad + " " + r.Hasta.Soyad,
                DoktorAd = r.Doktor.Ad + " " + r.Doktor.Soyad,
                BaslangicTarihi = r.BaslangicTarihi,
                BitisTarihi = r.BitisTarihi,
                Durum = r.Durum,
                Notlar = r.Notlar,
                ZoomJoinUrl = r.ZoomJoinUrl,
                ZoomHostUrl = r.ZoomHostUrl
            }).ToListAsync();
    }

    public async Task<RandevuResponseDto> GuncelleAsync(int randevuId, int kullaniciId, RandevuGuncelleDto dto)
    {
        var randevu = await _context.Randevular.FindAsync(randevuId)
            ?? throw new Exception("Randevu bulunamadı.");

        if (randevu.DoktorId != kullaniciId)
            throw new UnauthorizedAccessException("Bu randevuyu güncelleme yetkiniz yok.");

        randevu.Durum = dto.Durum;
        randevu.Notlar = dto.Notlar ?? randevu.Notlar;

        await _context.SaveChangesAsync();
        return await RandevuDonusturAsync(randevu.Id);
    }

    public async Task SilAsync(int randevuId, int kullaniciId)
    {
        var randevu = await _context.Randevular.FindAsync(randevuId)
            ?? throw new Exception("Randevu bulunamadı.");

        if (randevu.HastaId != kullaniciId && randevu.DoktorId != kullaniciId)
            throw new UnauthorizedAccessException("Bu randevuyu silme yetkiniz yok.");

        _context.Randevular.Remove(randevu);
        await _context.SaveChangesAsync();
    }

    private async Task<RandevuResponseDto> RandevuDonusturAsync(int id)
    {
        return await _context.Randevular
            .Include(r => r.Hasta)
            .Include(r => r.Doktor)
            .Where(r => r.Id == id)
            .Select(r => new RandevuResponseDto
            {
                Id = r.Id,
                HastaId = r.HastaId,
                DoktorId = r.DoktorId,
                HastaAd = r.Hasta.Ad + " " + r.Hasta.Soyad,
                DoktorAd = r.Doktor.Ad + " " + r.Doktor.Soyad,
                BaslangicTarihi = r.BaslangicTarihi,
                BitisTarihi = r.BitisTarihi,
                Durum = r.Durum,
                Notlar = r.Notlar,
                ZoomJoinUrl = r.ZoomJoinUrl,
                ZoomHostUrl = r.ZoomHostUrl
            }).FirstAsync();
    }

    public async Task<List<Randevu>> DoktorRandevulariAsync(int doktorId, DateTime tarih)
    {
        return await _context.Randevular
        .Where(r => r.DoktorId == doktorId &&
                    r.BaslangicTarihi.Date == tarih.Date)
        .ToListAsync();
    }
}