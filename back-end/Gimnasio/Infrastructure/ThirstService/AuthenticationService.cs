// Infrastructure/ThirstService/AuthenticationService.cs
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Application.Models.Helpers;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure.ThirstService
{
    public class AuthenticationService : IAuthService
    {
        private readonly AuthenticationServiceOptions _options;

        public AuthenticationService(IOptions<AuthenticationServiceOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("userType", user.UserType.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_options.SecretForKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}