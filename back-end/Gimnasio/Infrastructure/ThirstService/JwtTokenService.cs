// Infrastructure/ThirstService/JwtTokenService.cs
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.ThirstService;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _cfg;
    public JwtTokenService(IConfiguration cfg) => _cfg = cfg;

    public string GenerateToken(User user)
    {
        var issuer = _cfg["Jwt:Issuer"];
        var audience = _cfg["Jwt:Audience"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.NameAccount ?? user.Email),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.UserType.ToString())
        };

        var expires = DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:ExpiresMinutes"] ?? "120"));

        var token = new JwtSecurityToken(issuer, audience, claims, expires: expires, signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
