using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Entregas.Queries.GetEntrega;

public class GetEntregaHandler(IEntregaRepository repo) : IRequestHandler<GetEntregaQuery, EntregaDto>
{
    public async Task<EntregaDto> Handle(GetEntregaQuery request, CancellationToken ct)
    {
        var e = await repo.GetByPedidoAsync(request.PedidoId, ct)
            ?? throw new KeyNotFoundException($"Entrega for pedido {request.PedidoId} not found");
        return new EntregaDto(e.Id, e.PedidoId, e.Status.ToString(),
            e.TimestampSaiu, e.TimestampTransporte, e.TimestampEntregue);
    }
}
