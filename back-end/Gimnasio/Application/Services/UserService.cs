// Application/Services/UserService.cs
using Application.Interfaces;
using Application.Models.Request;
using Application.Models.Response;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenService _jwt;
    private readonly PasswordHasher<User> _hasher = new();

    public UserService(
        IUserRepository users,
        IUnitOfWork uow,
        IJwtTokenService jwt

    )
    {
        _users = users;
        _uow = uow;
        _jwt = jwt;
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static UserResponse ToResponse(User e) => new UserResponse
    {
        Id = e.Id,
        NameAccount = e.NameAccount,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Email = e.Email,
        Phone = e.Phone ?? string.Empty,
        Address = e.Address ?? string.Empty,
        Available = e.Available,
        UserType = e.UserType
    };

    public async Task<UserResponse> CreateAsync(UserRequest req, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            throw new ArgumentException("Email requerido");
        if (string.IsNullOrWhiteSpace(req.Password))
            throw new ArgumentException("Password requerido");

        var normEmail = NormalizeEmail(req.Email);
        if (await _users.EmailExistsAsync(normEmail, null, ct))
            throw new InvalidOperationException("Email ya registrado");

        // Siempre Customer y Available = true (defaults en la entidad + ctor de Customer)
        var entity = new Customer
        {
            NameAccount = req.NameAccount ?? string.Empty,
            FirstName = req.FirstName ?? string.Empty,
            LastName = req.LastName ?? string.Empty,
            Email = normEmail,
            Phone = string.IsNullOrWhiteSpace(req.Phone) ? null : req.Phone,
            Address = string.IsNullOrWhiteSpace(req.Address) ? null : req.Address
        };

        entity.PasswordHash = _hasher.HashPassword(entity, req.Password!);

        await _users.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return ToResponse(entity);
    }

    public async Task<UserResponse> UpdateAsync(int id, UserRequest req, CancellationToken ct = default)
    {
        var entity = await _users.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Usuario no encontrado");

        // Si está deshabilitado, no puede modificarse
        if (!entity.Available)
            throw new InvalidOperationException("Usuario deshabilitado");

        // Cambiar email (si viene y no es el mismo)
        if (!string.IsNullOrWhiteSpace(req.Email))
        {
            var newEmail = NormalizeEmail(req.Email);
            if (newEmail != entity.Email)
            {
                var exists = await _users.EmailExistsAsync(newEmail, excludeId: id, ct);
                if (exists) throw new InvalidOperationException("Email ya registrado por otro usuario");
                entity.Email = newEmail;
            }
        }

        if (!string.IsNullOrWhiteSpace(req.NameAccount)) entity.NameAccount = req.NameAccount!;
        if (!string.IsNullOrWhiteSpace(req.FirstName)) entity.FirstName = req.FirstName!;
        if (!string.IsNullOrWhiteSpace(req.LastName)) entity.LastName = req.LastName!;
        if (!string.IsNullOrWhiteSpace(req.Phone)) entity.Phone = req.Phone!;
        if (!string.IsNullOrWhiteSpace(req.Address)) entity.Address = req.Address!;

        if (!string.IsNullOrWhiteSpace(req.Password))
            entity.PasswordHash = _hasher.HashPassword(entity, req.Password);

        _users.Update(entity);
        await _uow.SaveChangesAsync(ct);

        return ToResponse(entity);
    }

    public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _users.GetByIdAsync(id, ct);
        return entity is null ? null : ToResponse(entity);
    }

    public async Task<IReadOnlyList<UserResponse>> ListAsync(CancellationToken ct = default)
    {
        var list = await _users.ListAsync(ct);
        return list.Select(ToResponse).ToList();
    }

    public async Task SoftDeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _users.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Usuario no encontrado");

        entity.Available = false;
        _users.Update(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task HardDeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _users.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Usuario no encontrado");

        _users.Remove(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<(UserResponse User, string Token)> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var normEmail = NormalizeEmail(email);
        var entity = await _users.GetByEmailAsync(normEmail, ct)
                     ?? throw new UnauthorizedAccessException("Credenciales inválidas");

        if (!entity.Available)
            throw new UnauthorizedAccessException("Usuario deshabilitado");

        var result = _hasher.VerifyHashedPassword(entity, entity.PasswordHash ?? string.Empty, password);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Credenciales inválidas");

        var token = _jwt.GenerateToken(entity);
        return (ToResponse(entity), token);
    }

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default)
        => _users.EmailExistsAsync(NormalizeEmail(email), excludeId, ct);
}
