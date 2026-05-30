namespace MedUnit.Application.Dtos;

public class TibbiDosyaDto
{
    public int Id { get; set; }
    public int HastaId { get; set; }
    public string DosyaAdi { get; set; } = string.Empty;
    public string DosyaTipi { get; set; } = string.Empty;
    public long DosyaBoyutu { get; set; }
    public DateTime YuklemeTarihi { get; set; }
}
