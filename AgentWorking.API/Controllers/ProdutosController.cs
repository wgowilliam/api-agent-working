using AgentWorking.Application.Features.Produtos.Commands.CreateProduto;
using AgentWorking.Application.Features.Produtos.Commands.PatchProdutoStatus;
using AgentWorking.Application.Features.Produtos.Commands.UpdateProduto;
using AgentWorking.Application.Features.Produtos.Queries.GetProdutos;
using AgentWorking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? centroId, [FromQuery] string? produtorId, CancellationToken ct)
        => Ok(await mediator.Send(new GetProdutosQuery(centroId, produtorId), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProdutoCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProdutoRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new UpdateProdutoCommand(
            id, req.Nome, req.Categoria, req.Quantidade, req.Unidade,
            req.Preco, req.Safra, req.Cidade, req.Foto), ct));

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> PatchStatus(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new PatchProdutoStatusCommand(id), ct));
}

public record UpdateProdutoRequest(
    string Nome, Categoria Categoria, decimal Quantidade, UnidadeMedida Unidade,
    decimal Preco, string Safra, string Cidade, string? Foto);
