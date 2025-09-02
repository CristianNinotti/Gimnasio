// Application/Users/AuthService.cs
using Application.Interfaces;
using Domain.Entities;

public class AuthService : IAuthService
{
    private readonly IPasswordHashingService _hasher;
    private readonly IUserRepository _users;        // Abstracción de repositorio
    private readonly IJwtTokenService _jwt;         // Abstracción para emitir JWT

    public AuthService(IPasswordHashingService hasher, IUserRepository users, IJwtTokenService jwt)
    {
        _hasher = hasher; _users = users; _jwt = jwt;
    }

    public async Task<User> RegisterAsync(string email, string password /*, ...*/)
    {
        if (await _users.ExistsByEmailAsync(email)) throw new InvalidOperationException("Email en uso");

        var user = new User
        {
            Email = email,
            Available = true,
            UserType = UserType.User
            // ... otros campos
        };
        user.PasswordHash = _hasher.Hash(password);

        await _users.AddAsync(user);
        return user;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _users.GetByEmailAsync(email) ?? throw new UnauthorizedAccessException();
        if (!_hasher.Verify(user.PasswordHash, password)) throw new UnauthorizedAccessException();

        return _jwt.CreateToken(user.Id, user.UserType.ToString()); // “User” / “SuperAdmin”
    }

}