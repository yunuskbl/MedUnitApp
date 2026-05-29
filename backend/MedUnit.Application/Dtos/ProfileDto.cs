using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedUnit.Application.Dtos
{
    public class ProfilDto
    {
        public string AdSoyad { get; set; }
        public string Biyografi { get; set; }
        public decimal DanismanlikUcreti { get; set; }
        public List<string> UzmanlikAlanlari { get; set; }
        public List<SertifikaDto> Sertifikalar { get; set; }
        public List<CalismaSaatiDto> CalismaSaatleri { get; set; }
    }
}