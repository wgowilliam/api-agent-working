using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AgentWorking.Application.Features.Lotes.Commands.UpdateLote;
using AgentWorking.Application.Features.Lotes.Queries.GetCatalogo;
using AgentWorking.Application.Features.Lotes.Queries.GetLotes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LotesController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Comprador")]
    [HttpGet]
    public async Task<IActionResult> GetByComprador(CancellationToken ct)
    {
        var compradorId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        return Ok(await mediator.Send(new GetLotesQuery(compradorId), ct));
    }

    [HttpGet("catalogo")]
    public async Task<IActionResult> GetCatalogo([FromQuery] string? categoria, CancellationToken ct)
        => Ok(await mediator.Send(new GetCatalogoQuery(categoria), ct));

    [Authorize(Roles = "Comprador")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLoteRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new UpdateLoteCommand(id, req.Quantidade, req.PrecoVenda, req.Validade), ct));
}

public record UpdateLoteRequest(decimal Quantidade, decimal PrecoVenda, DateTime Validade);
