using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Vendas.Queries.GetVendas;

public class GetVendasHandler(IVendaRepository repo) : IRequestHandler<GetVendasQuery, List<VendaDto>>
{
    public async Task<List<VendaDto>> Handle(GetVendasQuery request, CancellationToken ct)
    {
        var vendas = await repo.GetByProdutorAsync(request.ProdutorId, ct);
        return vendas.Select(v => new VendaDto(
            v.Id, v.CompraId, v.PedidoId, v.ProdutorId, v.CompradorId,
            v.Quantidade, v.ValorTotal, v.DataVenda)).ToList();
    }
}
