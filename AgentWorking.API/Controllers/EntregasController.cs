using AgentWorking.Application.Features.Entregas.Commands.PatchEntregaStatus;
using AgentWorking.Application.Features.Entregas.Queries.GetEntrega;
using AgentWorking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EntregasController(IMediator mediator) : ControllerBase
{
    [HttpGet("{pedidoId:guid}")]
    public async Task<IActionResult> GetByPedido(Guid pedidoId, CancellationToken ct)
        => Ok(await mediator.Send(new GetEntregaQuery(pedidoId), ct));

    [HttpPatch("{pedidoId:guid}/status")]
    public async Task<IActionResult> PatchStatus(
        Guid pedidoId, [FromBody] PatchEntregaRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new PatchEntregaStatusCommand(pedidoId, req.NovoStatus), ct));
}

public record PatchEntregaRequest(StatusEntrega NovoStatus);
