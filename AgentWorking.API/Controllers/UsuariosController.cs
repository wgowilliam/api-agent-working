using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AgentWorking.Application.Features.Usuarios.Commands.CadastrarUsuario;
using AgentWorking.Application.Features.Usuarios.Commands.LoginUsuario;
using AgentWorking.Application.Features.Usuarios.Commands.LogoutUsuario;
using AgentWorking.Application.Features.Usuarios.Queries.GetUsuario;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController(IMediator mediator) : ControllerBase
{
    [HttpPost("cadastro")]
    public async Task<IActionResult> Cadastrar(
        [FromBody] CadastrarUsuarioCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginUsuarioCommand cmd, CancellationToken ct)
        => Ok(await mediator.Send(cmd, ct));

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
        var expClaim = User.FindFirstValue(JwtRegisteredClaimNames.Exp);

        if (jti is null || expClaim is null || !long.TryParse(expClaim, out var expUnix))
            return BadRequest("Token inválido.");

        var expiry = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
        await mediator.Send(new LogoutUsuarioCommand(jti, expiry), ct);
        return NoContent();
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetUsuarioQuery(id), ct));
}
