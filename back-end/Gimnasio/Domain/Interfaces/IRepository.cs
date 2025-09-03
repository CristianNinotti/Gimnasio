// Domain/Interfaces/IRepository.cs
using System.Linq.Expressions;

namespace Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
}
