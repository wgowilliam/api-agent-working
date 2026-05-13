using AgentWorking.Application.Features.Pedidos.Commands.CreatePedido;
using AgentWorking.Application.Features.Pedidos.Commands.CreatePedidoPersonalizado;
using AgentWorking.Application.Features.Pedidos.Commands.PatchPedidoStatus;
using AgentWorking.Application.Features.Pedidos.Queries.GetPedidos;
using AgentWorking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? compradorId, [FromQuery] string? clienteId, CancellationToken ct)
        => Ok(await mediator.Send(new GetPedidosQuery(compradorId, clienteId), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePedidoCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    [HttpPost("personalizado")]
    public async Task<IActionResult> CreatePersonalizado(
        [FromBody] CreatePedidoPersonalizadoCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> PatchStatus(
        Guid id, [FromBody] PatchStatusRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new PatchPedidoStatusCommand(id, req.NovoStatus, req.NovaDataEntrega), ct));
}

public record PatchStatusRequest(StatusPedido NovoStatus, DateTime? NovaDataEntrega);
