using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedUnit.Application.Dtos;

public class RandevuResponseDto
{
    public int Id { get; set; }
    public int HastaId { get; set; }
    public int DoktorId { get; set; }
    public string HastaAd { get; set; } = string.Empty;
    public string DoktorAd { get; set; } = string.Empty;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public string Durum { get; set; } = string.Empty;
    public string? Notlar { get; set; }
    public string? ZoomJoinUrl { get; set; }
    public string? ZoomHostUrl { get; set; }
}
