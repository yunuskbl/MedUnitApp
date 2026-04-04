using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedUnit.Domain.Entities;

namespace MedUnit.Domain.Interfaces;

    public interface IKullaniciRepository
    {
        Task<(Kullanici kullanici, string token)> KayitOlAsync(string ad, string soyad, string email, string sifre, string rol);
        Task<(Kullanici kullanici, string token)> GirisYapAsync(string email, string sifre);
    }



