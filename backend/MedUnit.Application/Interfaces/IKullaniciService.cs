using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedUnit.Application.Dtos;

namespace MedUnit.Application.Interfaces;

    public interface IKullaniciService
    {
        Task<AuthResponseDto> KayitOlAsync(KayitDto dto);
        Task<AuthResponseDto> GirisYapAsync(GirisDto dto);
    }

