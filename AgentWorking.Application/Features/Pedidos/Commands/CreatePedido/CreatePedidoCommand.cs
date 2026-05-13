using AgentWorking.Application.DTOs;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.CreatePedido;

public record CreatePedidoItemDto(Guid LoteId, string Nome, decimal Quantidade, decimal Preco);
public record CreatePedidoCommand(
    string ClienteId, string CompradorId, string Endereco,
    MetodoPagamento MetodoPagamento, List<CreatePedidoItemDto> Itens) : IRequest<PedidoDto>;
