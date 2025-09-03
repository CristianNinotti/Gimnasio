// Application/Interfaces/IJwtTokenService.cs
using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
