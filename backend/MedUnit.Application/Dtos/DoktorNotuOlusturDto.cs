namespace MedUnit.Application.Dtos;

public class DoktorNotuOlusturDto
{
    public int RandevuId { get; set; }
    public int HastaId { get; set; }
    public string Not { get; set; } = string.Empty;
    public string? Tani { get; set; }
}
