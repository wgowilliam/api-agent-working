using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AgentWorking.Application.Features.Compras.Commands.CreateCompra;
using AgentWorking.Application.Features.Compras.Queries.GetCompras;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Comprador")]
public class ComprasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var compradorId = ResolveUserId();
        return Ok(await mediator.Send(new GetComprasQuery(compradorId), ct));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ComprarRequest req, CancellationToken ct)
    {
        var compradorId = ResolveUserId();
        var cmd = new CreateCompraCommand(compradorId, req.ProdutoId, req.Quantidade, req.PrecoUnitario, req.PrecoVenda, req.Validade);
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetAll), result);
    }

    private string ResolveUserId() =>
        User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException();
}

public record ComprarRequest(
    Guid ProdutoId,
    decimal Quantidade,
    decimal PrecoUnitario,
    decimal PrecoVenda,
    DateTime Validade);
