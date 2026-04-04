using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedUnit.Application.Dtos;

namespace MedUnit.Application.Interfaces;

    public interface IRandevuService
    {
        Task<RandevuResponseDto> OlusturAsync(int hastaId, RandevuOlusturDto dto);
        Task<List<RandevuResponseDto>> ListeleAsync(int kullaniciId, string rol);
        Task<RandevuResponseDto> GuncelleAsync(int randevuId, int kullaniciId, RandevuGuncelleDto dto);
        Task SilAsync(int randevuId, int kullaniciId);
    }

