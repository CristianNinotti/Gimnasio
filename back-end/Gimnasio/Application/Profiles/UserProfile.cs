
// Application/Porfiles/UserProfile.cs
using Application.Models.Request;
using Application.Models.Response;
using Domain.Entities;


namespace Application.Profiles;

public static class UserProfile
{
    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    public static User ToNewEntity(this UserRequest req)
    {
        var e = new Customer
        {
            NameAccount = req.NameAccount ?? string.Empty,
            FirstName = req.FirstName ?? string.Empty,
            LastName = req.LastName ?? string.Empty,
            Email = NormalizeEmail(req.Email!)
        };
        if (!string.IsNullOrWhiteSpace(req.Phone)) e.Phone = req.Phone;
        if (!string.IsNullOrWhiteSpace(req.Address)) e.Address = req.Address;
        return e;
    }

    public static void ApplyUpdates(this UserRequest req, User e)
    {
        if (!string.IsNullOrWhiteSpace(req.NameAccount)) e.NameAccount = req.NameAccount!;
        if (!string.IsNullOrWhiteSpace(req.FirstName)) e.FirstName = req.FirstName!;
        if (!string.IsNullOrWhiteSpace(req.LastName)) e.LastName = req.LastName!;
        if (!string.IsNullOrWhiteSpace(req.Phone)) e.Phone = req.Phone!;
        if (!string.IsNullOrWhiteSpace(req.Address)) e.Address = req.Address!;
        if (!string.IsNullOrWhiteSpace(req.Email))
        {
            var newEmail = NormalizeEmail(req.Email);
            if (newEmail != e.Email) e.Email = newEmail; // la unicidad la valida el servicio
        }
    }

    public static UserResponse ToResponse(this User e) => new UserResponse
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
}


