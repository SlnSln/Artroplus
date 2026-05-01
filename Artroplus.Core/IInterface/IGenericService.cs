using System.Linq.Expressions;

namespace Artroplus.Core.IInterface;

/// <summary>
/// Genel servis operasyonları için generic servis arayüzü.
/// CLAUDE.md Kural 7: İş mantığı Controller'a yazılmaz, Service katmanına taşınır.
/// </summary>
public interface IGenericService<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}
