using AgentWorking.Application.Features.Notificacoes.Commands.PatchNotificacaoLida;
using AgentWorking.Application.Features.Notificacoes.Commands.PatchTodasLidas;
using AgentWorking.Application.Features.Notificacoes.Queries.GetNotificacoes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacoesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string usuarioId, CancellationToken ct)
        => Ok(await mediator.Send(new GetNotificacoesQuery(usuarioId), ct));

    [HttpPatch("{id:guid}/lida")]
    public async Task<IActionResult> MarcarLida(Guid id, CancellationToken ct)
    {
        await mediator.Send(new PatchNotificacaoLidaCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("lidas")]
    public async Task<IActionResult> MarcarTodasLidas([FromQuery] string usuarioId, CancellationToken ct)
    {
        await mediator.Send(new PatchTodasLidasCommand(usuarioId), ct);
        return NoContent();
    }
}
