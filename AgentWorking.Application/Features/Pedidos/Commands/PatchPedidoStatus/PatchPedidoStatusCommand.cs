using AgentWorking.Application.DTOs;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.PatchPedidoStatus;
public record PatchPedidoStatusCommand(Guid Id, StatusPedido NovoStatus, DateTime? NovaDataEntrega)
    : IRequest<PedidoDto>;
