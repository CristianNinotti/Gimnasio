// Web/Extensions/SecurityExtensions.cs
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Web.Extensions;

public static class SecurityExtensions
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        var jwt = config.GetSection("Jwt");
        var key = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var issuer = jwt["Issuer"];
        var audience = jwt["Audience"];

        // AUTENTICACIÓN (JWT)
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

        // AUTORIZACIÓN (POLÍTICAS)
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", p => p.RequireRole("SuperAdmin"));

            options.AddPolicy("SelfOrAdmin", policy =>
                policy.RequireAssertion(ctx =>
                {
                    // Admin pasa directo
                    if (ctx.User.IsInRole("SuperAdmin")) return true;

                    // Id del usuario en el token
                    var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId)) return false;

                    // Id {id} desde la ruta usando HttpContext
                    if (ctx.Resource is HttpContext http)
                    {
                        var routeId = http.Request.RouteValues["id"]?.ToString();
                        return routeId == userId;
                    }

                    return false;
                })
            );
        });

        return services;
    }
}
