// Application/Interfaces/IAuthService.cs
using Domain.Entities;
namespace Application.Interfaces;
public interface IAuthService
{
    Task<User> RegisterAsync(string email, string password, /* otros campos */);
    Task<string> LoginAsync(string email, string password); // devuelve JWT
}
