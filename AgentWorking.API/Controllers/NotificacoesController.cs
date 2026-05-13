using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AgentWorking.Application.Features.Notificacoes.Commands.PatchNotificacaoLida;
using AgentWorking.Application.Features.Notificacoes.Commands.PatchTodasLidas;
using AgentWorking.Application.Features.Notificacoes.Queries.GetNotificacoes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacoesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        return Ok(await mediator.Send(new GetNotificacoesQuery(userId), ct));
    }

    [HttpPatch("{id:guid}/lida")]
    public async Task<IActionResult> MarcarLida(Guid id, CancellationToken ct)
    {
        await mediator.Send(new PatchNotificacaoLidaCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("lidas")]
    public async Task<IActionResult> MarcarTodasLidas(CancellationToken ct)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        await mediator.Send(new PatchTodasLidasCommand(userId), ct);
        return NoContent();
    }
}
