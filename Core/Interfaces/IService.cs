namespace Core.Interfaces;



public interface IService<T> where T : class
{
    Task<T> GetAsync(Guid id, CancellationToken token);

    Task<IEnumerable<T>> GetAllAsync(CancellationToken token);

    Task DeleteAsync(Guid id, CancellationToken token);

    Task<T> CreateAsync(T entity, CancellationToken token);

    Task<T> UpdateAsync(T entity, CancellationToken token);
}