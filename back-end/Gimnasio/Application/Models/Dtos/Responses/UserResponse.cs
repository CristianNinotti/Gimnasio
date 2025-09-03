// Application/Models/Response/UserResponse.cs
using Domain.Entities.Enum;

namespace Application.Models.Response;

public class UserResponse
{
    public int Id { get; set; }
    public string NameAccount { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool Available { get; set; }
    public UserType UserType { get; set; }
}
