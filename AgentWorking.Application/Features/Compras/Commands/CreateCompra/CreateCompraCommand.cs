using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Compras.Commands.CreateCompra;
public record CreateCompraCommand(
    string CompradorId, Guid ProdutoId,
    decimal Quantidade, decimal PrecoUnitario, decimal PrecoVenda,
    DateTime Validade) : IRequest<CompraDto>;
