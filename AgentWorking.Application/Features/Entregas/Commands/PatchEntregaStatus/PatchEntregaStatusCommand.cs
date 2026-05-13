using AgentWorking.Application.DTOs;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Entregas.Commands.PatchEntregaStatus;
public record PatchEntregaStatusCommand(Guid PedidoId, StatusEntrega NovoStatus) : IRequest<EntregaDto>;
