namespace MedUnit.Application.Dtos;

public class DoktorNotuDto
{
    public int Id { get; set; }
    public int RandevuId { get; set; }
    public string DoktorAdSoyad { get; set; } = string.Empty;
    public string HastaAdSoyad { get; set; } = string.Empty;
    public string Not { get; set; } = string.Empty;
    public string? Tani { get; set; }
    public DateTime OlusturulmaTarihi { get; set; }
}
