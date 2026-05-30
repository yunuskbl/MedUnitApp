using MedUnit.Application.Dtos;

namespace MedUnit.Application.Interfaces;

public interface IKullaniciService
{
    Task<AuthResponseDto> KayitOlAsync(KayitDto dto);
    Task<AuthResponseDto> GirisYapAsync(GirisDto dto);
    Task<AuthResponseDto> ProfilGuncelleAsync(int kullaniciId, ProfilGuncelleDto dto);
    Task SifreDegistirAsync(int kullaniciId, SifreDegistirDto dto);
    Task<ProfilDto> GetProfilAsync(string kullaniciId);
    Task UpdateProfilAsync(string kullaniciId, ProfilDto profil);
}
