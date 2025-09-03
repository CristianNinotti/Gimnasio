// Application/Services/UserService.cs
using Application.Interfaces;
using Application.Models.Request;
using Application.Models.Response;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IJwtTokenService _jwt;
    private readonly PasswordHasher<User> _hasher = new();

    public UserService(IUserRepository users, IUnitOfWork uow, IMapper mapper, IJwtTokenService jwt)
    {
        _users = users;
        _uow = uow;
        _mapper = mapper;
        _jwt = jwt;
    }

    // 🔹 Helper: normalizar email en un solo lugar
    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    public async Task<UserResponse> CreateAsync(UserRequest req, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(req.Email)) throw new ArgumentException("Email requerido");
        if (string.IsNullOrWhiteSpace(req.Password)) throw new ArgumentException("Password requerido");

        var normEmail = NormalizeEmail(req.Email);
        if (await _users.EmailExistsAsync(normEmail, null, ct))
            throw new InvalidOperationException("Email ya registrado");

        // ✅ Elegimos subtipo según el enum
        User entity = (req.UserType ?? UserType.Customer) switch
        {
            UserType.SuperAdmin => new SuperAdmin(),
            _ => new Customer()
        };

        // Seteo explícito
        entity.NameAccount = req.NameAccount ?? entity.NameAccount;
        entity.FirstName = req.FirstName ?? entity.FirstName;
        entity.LastName = req.LastName ?? entity.LastName;
        entity.Email = normEmail;
        entity.Phone = req.Phone ?? entity.Phone;
        entity.Address = req.Address ?? entity.Address;
        entity.Available = req.Available ?? true;
        entity.UserType = req.UserType ?? UserType.Customer;

        entity.PasswordHash = _hasher.HashPassword(entity, req.Password!);

        await _users.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<UserResponse>(entity);
    }

    public async Task<UserResponse> UpdateAsync(int id, UserRequest req, CancellationToken ct = default)
    {
        var entity = await _users.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Usuario no encontrado");

        // 🔹 Permitir cambiar email si viene y es distinto (normalizado)
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

        if (!string.IsNullOrWhiteSpace(req.FirstName)) entity.FirstName = req.FirstName;
        if (!string.IsNullOrWhiteSpace(req.LastName)) entity.LastName = req.LastName;
        if (!string.IsNullOrWhiteSpace(req.Phone)) entity.Phone = req.Phone;
        if (!string.IsNullOrWhiteSpace(req.Address)) entity.Address = req.Address;
        if (!string.IsNullOrWhiteSpace(req.NameAccount)) entity.NameAccount = req.NameAccount;

        if (req.Available.HasValue) entity.Available = req.Available.Value;
        if (req.UserType.HasValue) entity.UserType = req.UserType.Value;

        if (!string.IsNullOrWhiteSpace(req.Password))
            entity.PasswordHash = _hasher.HashPassword(entity, req.Password);

        _users.Update(entity);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<UserResponse>(entity);
    }

    public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _users.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<UserResponse>(entity);
    }

    public async Task<IReadOnlyList<UserResponse>> ListAsync(CancellationToken ct = default)
    {
        var list = await _users.ListAsync(ct);
        return list.Select(_mapper.Map<UserResponse>).ToList();
    }

    public async Task SoftDeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _users.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Usuario no encontrado");
        entity.Available = false;
        _users.Update(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task HardDeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _users.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Usuario no encontrado");
        _users.Remove(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<(UserResponse User, string Token)> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var normEmail = NormalizeEmail(email);
        var entity = await _users.GetByEmailAsync(normEmail, ct)
                     ?? throw new UnauthorizedAccessException("Credenciales inválidas");

        var result = _hasher.VerifyHashedPassword(entity, entity.PasswordHash ?? string.Empty, password);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Credenciales inválidas");

        var token = _jwt.GenerateToken(entity);
        var userDto = _mapper.Map<UserResponse>(entity);
        return (userDto, token);
    }

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default)
        => _users.EmailExistsAsync(NormalizeEmail(email), excludeId, ct);
}
