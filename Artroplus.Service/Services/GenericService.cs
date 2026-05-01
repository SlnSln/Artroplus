using System.Linq.Expressions;
using Artroplus.Core.IInterface;
using Artroplus.Core.IRepositories;

namespace Artroplus.Service.Services;

/// <summary>
/// IGenericService implementasyonu.
/// CLAUDE.md Kural 7: İş mantığı bu katmanda yer alır, Controller'a yazılmaz.
/// </summary>
public class GenericService<T> : IGenericService<T> where T : class
{
    private readonly IGenericRepository<T> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public GenericService(IGenericRepository<T> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _repository.FindAsync(predicate);
    }

    public async Task<T> AddAsync(T entity)
    {
        var added = await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return added;
    }

    public async Task UpdateAsync(T entity)
    {
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveAsync(T entity)
    {
        _repository.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        return await _repository.CountAsync(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _repository.AnyAsync(predicate);
    }
}
