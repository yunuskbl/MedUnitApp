using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedUnit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedUnit.Application.Interfaces;

    public interface IAppDbContext
    {
        DbSet<Kullanici> Kullanicilar { get; }
        DbSet<Randevu> Randevular { get; }
        DbSet<Klinik> Klinikler { get; }
        DbSet<DoktorNotu> DoktorNotlari { get; }
        DbSet<TibbiDosya> TibbiDosyalar { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

