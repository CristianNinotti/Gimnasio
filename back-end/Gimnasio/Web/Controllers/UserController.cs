using Application.Interfaces;
using Application.Models.Request;
using Application.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _svc;
    public UsersController(IUserService svc) => _svc = svc;

    // GET libre o protegido, como prefieras. Lo dejo protegido:
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> GetById(int id, CancellationToken ct)
    {
        var u = await _svc.GetByIdAsync(id, ct);
        return u is null ? NotFound() : Ok(u);
    }

    [HttpGet]
    [Authorize(Roles = "SuperAdmin")] // opcional: solo admin ve todos
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> List(CancellationToken ct)
        => Ok(await _svc.ListAsync(ct));

    // POST (registro) LIBERADO
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponse>> Create([FromBody] UserRequest req, CancellationToken ct)
    {
        var created = await _svc.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // UPDATE: solo el dueño (id de ruta) o SuperAdmin
    [HttpPatch("{id:int}")]
    [Authorize(Policy = "SelfOrAdmin")]
    public async Task<ActionResult<UserResponse>> Update(int id, [FromBody] UserRequest req, CancellationToken ct)
        => Ok(await _svc.UpdateAsync(id, req, ct));

    // SOFT DELETE: solo el dueño o SuperAdmin (pone Available=false)
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "SelfOrAdmin")]
    public async Task<IActionResult> SoftDelete(int id, CancellationToken ct)
    {
        await _svc.SoftDeleteAsync(id, ct);
        return NoContent();
    }

    // HARD DELETE: solo SuperAdmin
    [HttpDelete("{id:int}/hard")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> HardDelete(int id, CancellationToken ct)
    {
        await _svc.HardDeleteAsync(id, ct);
        return NoContent();
    }
}
