namespace MedUnit.Domain.Entities;

public class Klinik
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string? Adres { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? LogoUrl { get; set; }
    public string AbonelikTipi { get; set; } = "ucretsiz"; // ucretsiz | temel | premium
    public DateTime? AbonelikBitisTarihi { get; set; }
    public bool Aktif { get; set; } = true;
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
}
