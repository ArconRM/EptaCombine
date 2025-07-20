using Core.Interfaces;

namespace Core.BaseEntities;

public class BaseService<T> : IService<T> where T : class
{
    private readonly IRepository<T> _repository;

    public BaseService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public async Task<T> GetAsync(Guid id, CancellationToken token)
    {
        return await _repository.GetAsync(id, token);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken token)
    {
        return await _repository.GetAllAsync(token);
    }

    public async Task DeleteAsync(Guid id, CancellationToken token)
    {
        await _repository.DeleteAsync(id, token);
    }

    public async Task<T> CreateAsync(T entity, CancellationToken token)
    {
        return await _repository.CreateAsync(entity, token);
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken token)
    {
        return await _repository.UpdateAsync(entity, token);
    }
}