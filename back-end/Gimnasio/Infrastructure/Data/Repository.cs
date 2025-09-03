// Infrastructure/Data/Repository.cs
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Data;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _db;
    protected readonly DbSet<T> _set;

    public Repository(DbContext db)
    {
        _db = db;
        _set = _db.Set<T>();
    }

    public virtual Task<T?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _set.FindAsync(new object?[] { id }, ct).AsTask();

    public virtual async Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default) =>
        await _set.AsNoTracking().ToListAsync(ct);

    public virtual async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await _set.AsNoTracking().Where(predicate).ToListAsync(ct);

    public virtual Task AddAsync(T entity, CancellationToken ct = default) =>
        _set.AddAsync(entity, ct).AsTask();

    public virtual void Update(T entity) => _set.Update(entity);

    public virtual void Remove(T entity) => _set.Remove(entity);

    public virtual Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        _set.AnyAsync(predicate, ct);
}
