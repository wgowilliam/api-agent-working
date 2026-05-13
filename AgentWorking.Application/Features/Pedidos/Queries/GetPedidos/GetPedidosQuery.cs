using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Queries.GetPedidos;
public record GetPedidosQuery(string? CompradorId, string? ClienteId) : IRequest<List<PedidoDto>>;
