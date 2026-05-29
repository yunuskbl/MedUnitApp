namespace MedUnit.Domain.Entities;

public class TibbiDosya
{
    public int Id { get; set; }
    public int HastaId { get; set; }
    public Kullanici Hasta { get; set; } = null!;
    public string DosyaAdi { get; set; } = string.Empty;
    public string DosyaTipi { get; set; } = string.Empty;
    public byte[] DosyaVerisi { get; set; } = Array.Empty<byte>();
    public long DosyaBoyutu { get; set; }
    public DateTime YuklemeTarihi { get; set; } = DateTime.UtcNow;
}
