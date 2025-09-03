// Domain/Entities/User.cs
using Domain.Entities.Enum;

namespace Domain.Entities;

public abstract class User
{
    public int Id { get; set; }

    // Recomendado como obligatorios:
    public string NameAccount { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Opcionales:
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public bool Available { get; set; } = true;
    public UserType UserType { get; set; } = UserType.Customer;

    public string PasswordHash { get; set; } = string.Empty; // evito nulls
}
