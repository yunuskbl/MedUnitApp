using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedUnit.Application.Dtos
{
    public class CalismaSaatiDto
    {
        public string Gun { get; set; }
        public string BaslangicSaati { get; set; }
        public string BitisSaati { get; set; }
        public bool Izinli { get; set; }
    }
}
