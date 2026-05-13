using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AgentWorking.Application.Features.Pedidos.Commands.CreatePedido;
using AgentWorking.Application.Features.Pedidos.Commands.CreatePedidoPersonalizado;
using AgentWorking.Application.Features.Pedidos.Commands.PatchPedidoStatus;
using AgentWorking.Application.Features.Pedidos.Queries.GetPedidos;
using AgentWorking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        return Ok(await mediator.Send(new GetPedidosQuery(userId, null), ct));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarPedidoRequest req, CancellationToken ct)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        var cmd = new CreatePedidoCommand(userId, userId, req.Endereco, req.MetodoPagamento, req.Itens);
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetAll), result);
    }

    [HttpPost("personalizado")]
    public async Task<IActionResult> CreatePersonalizado(
        [FromBody] CreatePedidoPersonalizadoCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetAll), result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> PatchStatus(
        Guid id, [FromBody] PatchStatusRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new PatchPedidoStatusCommand(id, req.NovoStatus, req.NovaDataEntrega), ct));
}

public record CriarPedidoRequest(
    string Endereco,
    MetodoPagamento MetodoPagamento,
    List<CreatePedidoItemDto> Itens);

public record PatchStatusRequest(StatusPedido NovoStatus, DateTime? NovaDataEntrega);
