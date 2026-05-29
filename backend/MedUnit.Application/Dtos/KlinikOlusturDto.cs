namespace MedUnit.Application.Dtos;

public class KlinikOlusturDto
{
    public string Ad { get; set; } = string.Empty;
    public string? Adres { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string AbonelikTipi { get; set; } = "ucretsiz";
}
