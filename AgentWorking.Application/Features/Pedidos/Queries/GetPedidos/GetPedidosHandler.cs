using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Queries.GetPedidos;

public class GetPedidosHandler(IPedidoRepository repo) : IRequestHandler<GetPedidosQuery, List<PedidoDto>>
{
    public async Task<List<PedidoDto>> Handle(GetPedidosQuery request, CancellationToken ct)
    {
        var pedidos = !string.IsNullOrEmpty(request.CompradorId)
            ? await repo.GetByCompradorAsync(request.CompradorId, ct)
            : await repo.GetByClienteAsync(request.ClienteId ?? string.Empty, ct);

        return pedidos.Select(ToDto).ToList();
    }

    private static PedidoDto ToDto(Pedido p)
    {
        var pp = p as PedidoPersonalizado;
        return new PedidoDto(
            p.Id, p.Tipo.ToString(), p.ClienteId, p.CompradorId, p.Endereco,
            p.Status.ToString(), p.DataCriacao, p.DataEntrega,
            p.MetodoPagamento?.ToString(), p.StatusPagamento,
            p.Itens.Select(i => new ItemPedidoDto(i.Id, i.LoteId, i.Nome, i.Quantidade, i.Preco)).ToList(),
            pp?.Especie, pp?.QuantidadeTotal, pp?.DataLimite, pp?.Observacoes, pp?.PrazoAceite);
    }
}
