// Application/Interfaces/IUserService.cs
using Application.Models.Request;
using Application.Models.Response;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> CreateAsync(UserRequest req, CancellationToken ct = default);
        Task<UserResponse> UpdateAsync(int id, UserRequest req, CancellationToken ct = default);
        Task<UserResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<UserResponse>> ListAsync(CancellationToken ct = default);
        Task SoftDeleteAsync(int id, CancellationToken ct = default);
        Task HardDeleteAsync(int id, CancellationToken ct = default);

        Task<(UserResponse User, string Token)> LoginAsync(string email, string password, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default);
    }
}
