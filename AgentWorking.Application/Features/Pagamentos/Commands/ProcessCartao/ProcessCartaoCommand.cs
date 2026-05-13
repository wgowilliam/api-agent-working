using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessCartao;
public record ProcessCartaoCommand(
    Guid PedidoId, string NumeroCartao, string NomeTitular,
    string Validade, string Cvv, decimal Valor) : IRequest<PagamentoCartaoDto>;
