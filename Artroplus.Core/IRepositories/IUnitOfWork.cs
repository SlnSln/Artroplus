namespace Artroplus.Core.IRepositories;

/// <summary>
/// Unit of Work arayüzü — tüm repository'leri tek bir transaction kapsamında yönetir.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
}
