// Infrastructure/Data/UserRepository.cs
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly DbSet<User> _users;

    public UserRepository(DbContext db) : base(db)
    {
        _users = db.Set<User>();
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        _users.AsNoTracking()
              .FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower(), ct);

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default) =>
        _users.AnyAsync(u =>
            u.Email == email.Trim().ToLower() &&
            (excludeId == null || u.Id != excludeId),
            ct);

    public Task<TUser?> GetByIdAsync<TUser>(int id, CancellationToken ct = default) where TUser : User =>
        _db.Set<TUser>().AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<IReadOnlyList<TUser>> ListByTypeAsync<TUser>(CancellationToken ct = default) where TUser : User =>
        await _db.Set<TUser>().AsNoTracking().ToListAsync(ct);
}
