// Application/Models/Request/UserRequest.cs
using Domain.Entities.Enum;

namespace Application.Models.Request;

public class UserRequest
{
    // Datos básicos
    public string? NameAccount { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }

    // Estado / Rol
    public bool? Available { get; set; }
    public UserType? UserType { get; set; }

    // Seguridad
    public string? Password { get; set; } // alta / login / cambio de clave
}
