using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MedUnit.Domain.Entities;

public class Kullanici
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SifreHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "hasta";   // hasta | doktor | admin
    public bool Aktif { get; set; } = true;
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
    public string? Biyografi { get; set; }
    public decimal DanismanlikUcreti { get; set; } = 0;
    public string SertifikalarJson{ get; set; } = "[]";

    [NotMapped]
    public List<Sertifika> Sertifikalar
    {
        get => JsonSerializer.Deserialize<List<Sertifika>>(SertifikalarJson) ?? new();
        set => SertifikalarJson = JsonSerializer.Serialize(value);
    }

    // Liste alanları JSON string olarak tutulur
    public string UzmanlikAlanlariJson { get; set; } = "[]";
    public string CalismaSaatleriJson { get; set; } = "[]";

    // Entity'de JSON'ı parse eden yardımcı property'ler
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
public class CalismaSaati // Çalışma saatlerini temsil eden sınıf
{
    public string Gun { get; set; } = string.Empty;
    public string BaslangicSaati { get; set; } = string.Empty;
    public string BitisSaati { get; set; } = string.Empty;
    public bool Izinli { get; set; }
}
public class Sertifika // Sertifikaları temsil eden sınıf
{
    public string Baslik { get; set; } = string.Empty;
    public string Kurum { get; set; } = string.Empty;
    public int Yil { get; set; }
    public string Ikon { get; set; } = string.Empty;
}

