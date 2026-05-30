namespace MedUnit.Domain.Entities;

public class DoktorNotu
{
    public int Id { get; set; }
    public int RandevuId { get; set; }
    public Randevu Randevu { get; set; } = null!;
    public int DoktorId { get; set; }
    public Kullanici Doktor { get; set; } = null!;
    public int HastaId { get; set; }
    public Kullanici Hasta { get; set; } = null!;
    public string Not { get; set; } = string.Empty;
    public string? Tani { get; set; }
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
}
