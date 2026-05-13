using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessBoleto;
public record ProcessBoletoCommand(Guid PedidoId, decimal Valor) : IRequest<PagamentoBoletoDto>;
