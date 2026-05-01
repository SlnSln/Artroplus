using System.Linq.Expressions;
using Artroplus.Core.IRepositories;
using Artroplus.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Artroplus.Data.Repositories;

/// <summary>
/// IGenericRepository implementasyonu.
/// CLAUDE.md Kural 7: Salt okunur sorgularda .AsNoTracking() kullanılır.
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ArtroplusDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ArtroplusDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        var list = entities.ToList();
        await _dbSet.AddRangeAsync(list);
        return list;
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        return predicate is null
            ? await _dbSet.CountAsync()
            : await _dbSet.CountAsync(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}
