using AgentWorking.Application.Features.Pagamentos.Commands.ProcessBoleto;
using AgentWorking.Application.Features.Pagamentos.Commands.ProcessCartao;
using AgentWorking.Application.Features.Pagamentos.Commands.ProcessPix;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagamentosController(IMediator mediator) : ControllerBase
{
    [HttpPost("pix")]
    public async Task<IActionResult> Pix([FromBody] ProcessPixCommand cmd, CancellationToken ct)
        => Ok(await mediator.Send(cmd, ct));

    [HttpPost("cartao")]
    public async Task<IActionResult> Cartao([FromBody] ProcessCartaoCommand cmd, CancellationToken ct)
        => Ok(await mediator.Send(cmd, ct));

    [HttpPost("boleto")]
    public async Task<IActionResult> Boleto([FromBody] ProcessBoletoCommand cmd, CancellationToken ct)
        => Ok(await mediator.Send(cmd, ct));
}
