// Application/Security/IPasswordHashingService.cs
namespace Application.Interfaces;
public interface IPasswordHashingService
{
    string Hash(string plainPassword);
    bool Verify(string hashedPassword, string plainPassword);
}
