using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Compras.Queries.GetCompras;

public class GetComprasHandler(ICompraRepository repo) : IRequestHandler<GetComprasQuery, List<CompraDto>>
{
    public async Task<List<CompraDto>> Handle(GetComprasQuery request, CancellationToken ct)
    {
        var compras = await repo.GetByCompradorAsync(request.CompradorId, ct);
        return compras.Select(c => new CompraDto(
            c.Id, c.ProdutorId, c.CompradorId, c.ProdutoId,
            c.Produto?.Nome ?? string.Empty, c.Quantidade,
            c.PrecoUnitario, c.DataCompra, c.Lote?.Id)).ToList();
    }
}
