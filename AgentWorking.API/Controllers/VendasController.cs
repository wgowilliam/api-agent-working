using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AgentWorking.Application.Features.Vendas.Queries.GetVendas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Produtor")]
public class VendasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var produtorId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        return Ok(await mediator.Send(new GetVendasQuery(produtorId), ct));
    }
}
