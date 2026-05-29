namespace MedUnit.Application.Dtos;

public class KlinikDto
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string? Adres { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? LogoUrl { get; set; }
    public string AbonelikTipi { get; set; } = string.Empty;
    public DateTime? AbonelikBitisTarihi { get; set; }
    public bool Aktif { get; set; }
    public DateTime OlusturulmaTarihi { get; set; }
    public int UyeSayisi { get; set; }
}
