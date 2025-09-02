// Domain/Entities/User.cs
using Domain.Entities.Enum;

namespace Domain.Entities;

public abstract class User
{
    public int Id { get; set; }
    public string? NameAccount { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public bool Available { get; set; } = true;

    // Público: lo podés setear donde quieras
    public UserType UserType { get; set; } = UserType.Customer;
    public string? PasswordHash { get; set; }


}