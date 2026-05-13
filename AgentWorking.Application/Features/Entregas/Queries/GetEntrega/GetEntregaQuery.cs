using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Entregas.Queries.GetEntrega;
public record GetEntregaQuery(Guid PedidoId) : IRequest<EntregaDto>;
