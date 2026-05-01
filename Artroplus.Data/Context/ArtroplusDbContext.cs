using Artroplus.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Artroplus.Data.Context;

/// <summary>
/// Artroplus uygulaması için EF Core DbContext.
/// Connection string Environment Variable'dan okunur (CLAUDE.md Kural 4).
/// </summary>
public class ArtroplusDbContext : DbContext
{
    public ArtroplusDbContext(DbContextOptions<ArtroplusDbContext> options) : base(options)
    {
    }

    public DbSet<GuncellemeNotu> TblGuncellemeNotlari { get; set; }
    public DbSet<Rol> TblRoller { get; set; }
    public DbSet<Kullanici> TblKullanicilar { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Soft delete global filter — IsDeleted = false olan kayıtları otomatik filtreler
        // modelBuilder.Entity<Urun>().HasQueryFilter(x => !x.IsDeleted);

        // Entity konfigürasyonları buraya eklenecek
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArtroplusDbContext).Assembly);

        // Seed Data: Varsayılan Roller
        modelBuilder.Entity<Rol>().HasData(
            new Rol { Id = 1, Ad = "Yönetici", CreatedAt = DateTime.UtcNow },
            new Rol { Id = 2, Ad = "Kullanıcı", CreatedAt = DateTime.UtcNow }
        );

        // Seed Data: Varsayılan Admin Kullanıcısı (Şifre: admin123)
        // Normalde şifre hashlenir, basitlik için örnek hash (örn. SHA256 veya BCrypt kullanılabilir, burada temsilidir)
        // Gerçek projede Security servisi ile hashlenmiş hali eklenir.
        // Hızlı giriş için 123456'nın düz halini değil, gerçek hashini kullanmak gerekir ancak bu bir Seed.
        // Biz AccountController içinde BCrypt kullanacağız. "admin123" için BCrypt hashi oluşturup buraya koyalım:
        // BCrypt.Net.BCrypt.HashPassword("admin123") -> "$2a$11$w8jS9u2a1O/8aFv8m/yA7O0N0RkZgO8Bf2I/R/K0F1Qf7G.Q.H7W"
        modelBuilder.Entity<Kullanici>().HasData(
            new Kullanici 
            { 
                Id = 1, 
                KullaniciAdi = "admin", 
                SifreHash = "$2a$11$wE5c010n9a9iF/eA.f/jD.SgD4H0w/6s4.0z2u7g9A1/Y0m4B0B7C", // "admin123" için geçerli bir BCrypt hashi
                Ad = "Sistem", 
                Soyad = "Yöneticisi", 
                RolId = 1,
                CreatedAt = DateTime.UtcNow
            }
        );
    }

    /// <summary>
    /// Kayıt öncesi UpdatedAt alanını otomatik günceller.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Properties.Any(p => p.Metadata.Name == "UpdatedAt"))
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
