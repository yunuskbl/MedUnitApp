using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedUnit.Domain.Entities;

    public class Kullanici
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SifreHash { get; set; } = string.Empty;
        public string Rol { get; set; } = "hasta";   // hasta | doktor | admin
        public bool Aktif { get; set; } = true;
        public DateTime OlusturulmaTarihi { get; set; } =
        TimeZoneInfo.ConvertTimeFromUtc(
        DateTime.UtcNow,
        TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"));
    }

