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
    public DbSet<Klinik> Klinikler { get; set; }
    public DbSet<DoktorNotu> DoktorNotlari { get; set; }
    public DbSet<TibbiDosya> TibbiDosyalar { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Kullanici>()
            .HasIndex(k => k.Email)
            .IsUnique();

        modelBuilder.Entity<Kullanici>()
            .Property(k => k.Rol)
            .HasMaxLength(20);

        modelBuilder.Entity<Kullanici>()
            .HasOne(k => k.Klinik)
            .WithMany()
            .HasForeignKey(k => k.KlinikId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

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

        modelBuilder.Entity<DoktorNotu>()
            .HasOne(n => n.Doktor)
            .WithMany()
            .HasForeignKey(n => n.DoktorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DoktorNotu>()
            .HasOne(n => n.Hasta)
            .WithMany()
            .HasForeignKey(n => n.HastaId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DoktorNotu>()
            .HasOne(n => n.Randevu)
            .WithMany()
            .HasForeignKey(n => n.RandevuId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TibbiDosya>()
            .HasOne(d => d.Hasta)
            .WithMany()
            .HasForeignKey(d => d.HastaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}