using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedUnit.Domain.Entities;

namespace MedUnit.Domain.Interfaces;

    public interface IRandevuRepository
    {
        Task<object> OlusturAsync(int hastaId, object dto);
        Task<List<object>> ListeleAsync(int kullaniciId, string rol);
        Task<object> GuncelleAsync(int randevuId, int kullaniciId, object dto);
        Task SilAsync(int randevuId, int kullaniciId);
    }

