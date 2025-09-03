using System.Security.Claims;
using Application.Interfaces;
using Application.Models.Request; // LoginRequest (Email, Password)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _users;
    public AuthController(IUserService users) => _users = users;

    [HttpPost("login")]
    [AllowAnonymous]
    [Produces("text/plain")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var (_, token) = await _users.LoginAsync(req.NameAccount, req.Password, ct);

        // OPCIONAL: también te lo pongo en el header Authorization
        Response.Headers["Authorization"] = $"Bearer {token}";

        // ← Devuelve SOLO el token como texto plano (sin JSON)
        return Content(token, "text/plain");
    }
}
