// Domain/Interfaces/IUserRepository.cs
using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default);

    // Si necesitás trabajar por subtipo
    Task<TUser?> GetByIdAsync<TUser>(int id, CancellationToken ct = default) where TUser : User;
    Task<IReadOnlyList<TUser>> ListByTypeAsync<TUser>(CancellationToken ct = default) where TUser : User;
}
