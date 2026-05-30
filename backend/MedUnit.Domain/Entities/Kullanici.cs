using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace MedUnit.Domain.Entities;

public class Kullanici
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SifreHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "hasta";
    public string? Uzmanlik { get; set; }
    public bool Aktif { get; set; } = true;
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
    public string? Telefon { get; set; }
    public string? SifreSifirlamaToken { get; set; }
    public DateTime? SifreSifirlamaTokenSon { get; set; }
    public int? KlinikId { get; set; }
    public Klinik? Klinik { get; set; }
    public string? Biyografi { get; set; }
    public decimal DanismanlikUcreti { get; set; } = 0;
    public string SertifikalarJson { get; set; } = "[]";
    public string UzmanlikAlanlariJson { get; set; } = "[]";
    public string CalismaSaatleriJson { get; set; } = "[]";

    [NotMapped]
    public List<Sertifika> Sertifikalar
    {
        get => JsonSerializer.Deserialize<List<Sertifika>>(SertifikalarJson) ?? new();
        set => SertifikalarJson = JsonSerializer.Serialize(value);
    }

    [NotMapped]
    public List<string> UzmanlikAlanlari
    {
        get => JsonSerializer.Deserialize<List<string>>(UzmanlikAlanlariJson) ?? new();
        set => UzmanlikAlanlariJson = JsonSerializer.Serialize(value);
    }

    [NotMapped]
    public List<CalismaSaati> CalismaSaatleri
    {
        get => JsonSerializer.Deserialize<List<CalismaSaati>>(CalismaSaatleriJson) ?? new();
        set => CalismaSaatleriJson = JsonSerializer.Serialize(value);
    }
}

public class CalismaSaati
{
    public string Gun { get; set; } = string.Empty;
    public string BaslangicSaati { get; set; } = string.Empty;
    public string BitisSaati { get; set; } = string.Empty;
    public bool Izinli { get; set; }
}

public class Sertifika
{
    public string Baslik { get; set; } = string.Empty;
    public string Kurum { get; set; } = string.Empty;
    public int Yil { get; set; }
    public string Ikon { get; set; } = string.Empty;
}
