using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessPix;
public record ProcessPixCommand(Guid PedidoId, decimal Valor) : IRequest<PagamentoPixDto>;
