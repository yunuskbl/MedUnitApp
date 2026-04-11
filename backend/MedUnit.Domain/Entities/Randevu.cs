using MedUnit.Domain.Entities;

public class Randevu
{
    public int Id { get; set; }
    public int HastaId { get; set; }
    public Kullanici Hasta { get; set; } = null!;
    public int DoktorId { get; set; }
    public Kullanici Doktor { get; set; } = null!;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public string Durum { get; set; } = "beklemede";
    public string? Notlar { get; set; }
    public bool HatirlaticiGonderildi { get; set; } = false;
    public string? ZoomMeetingId { get; set; }
    public string? ZoomJoinUrl { get; set; }
    public string? ZoomHostUrl { get; set; }
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
}