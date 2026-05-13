using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.CreatePedidoPersonalizado;
public record CreatePedidoPersonalizadoCommand(
    string ClienteId, string CompradorId, string Endereco,
    string Especie, decimal QuantidadeTotal, DateTime DataLimite,
    string Observacoes) : IRequest<PedidoDto>;
