using AgentWorking.Application.Features.Vendas.Queries.GetVendas;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string produtorId, CancellationToken ct)
        => Ok(await mediator.Send(new GetVendasQuery(produtorId), ct));
}
