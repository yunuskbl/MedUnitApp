using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedUnit.Application.Dtos;

    public class RandevuGuncelleDto
    {
        public string Durum { get; set; } = string.Empty;
        public string? Notlar { get; set; }
    }

