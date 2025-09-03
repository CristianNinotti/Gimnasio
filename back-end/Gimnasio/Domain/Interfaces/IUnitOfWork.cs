// Domain/Interfaces/IUnitOfWork.cs
namespace Domain.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
