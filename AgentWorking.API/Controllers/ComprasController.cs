using AgentWorking.Application.Features.Compras.Commands.CreateCompra;
using AgentWorking.Application.Features.Compras.Queries.GetCompras;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComprasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string compradorId, CancellationToken ct)
        => Ok(await mediator.Send(new GetComprasQuery(compradorId), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompraCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetAll), new { compradorId = cmd.CompradorId }, result);
    }
}
