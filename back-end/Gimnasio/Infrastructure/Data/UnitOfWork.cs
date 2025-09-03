// Infrastructure/Data/UnitOfWork.cs
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _db;
    public UnitOfWork(DbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
