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
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

