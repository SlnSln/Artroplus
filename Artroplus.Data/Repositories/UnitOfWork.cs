using Artroplus.Core.IRepositories;
using Artroplus.Data.Context;

namespace Artroplus.Data.Repositories;

/// <summary>
/// Unit of Work implementasyonu.
/// Tüm repository'leri tek bir transaction kapsamında yönetir.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ArtroplusDbContext _context;
    private bool _disposed = false;

    public UnitOfWork(ArtroplusDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
