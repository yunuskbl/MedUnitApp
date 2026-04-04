using Microsoft.EntityFrameworkCore;
using MedUnit.Domain.Entities;
using MedUnit.Application.Interfaces;

namespace MedUnit.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Kullanici> Kullanicilar { get; set; }
    public DbSet<Randevu> Randevular { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Kullanici>()
            .HasIndex(k => k.Email)
            .IsUnique();

        modelBuilder.Entity<Kullanici>()
            .Property(k => k.Rol)
            .HasMaxLength(20);

        modelBuilder.Entity<Randevu>()
            .HasOne(r => r.Hasta)
            .WithMany()
            .HasForeignKey(r => r.HastaId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Randevu>()
            .HasOne(r => r.Doktor)
            .WithMany()
            .HasForeignKey(r => r.DoktorId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}