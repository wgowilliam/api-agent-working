using AgentWorking.Application.Features.Usuarios.Commands.CadastrarUsuario;
using AgentWorking.Application.Features.Usuarios.Queries.GetUsuario;
using MediatR;
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetUsuarioQuery(id), ct));
}
