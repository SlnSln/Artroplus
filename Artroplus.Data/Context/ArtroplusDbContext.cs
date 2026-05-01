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

    // DbSet'ler buraya eklenecek
    // Örnek: public DbSet<Urun> Urunler { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Soft delete global filter — IsDeleted = false olan kayıtları otomatik filtreler
        // modelBuilder.Entity<Urun>().HasQueryFilter(x => !x.IsDeleted);

        // Entity konfigürasyonları buraya eklenecek
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArtroplusDbContext).Assembly);
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
