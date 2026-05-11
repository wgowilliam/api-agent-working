using AgentWorking.Application.DTOs;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Commands.UpdateProduto;
public record UpdateProdutoCommand(
    Guid Id, string Nome, Categoria Categoria, decimal Quantidade,
    UnidadeMedida Unidade, decimal Preco, string Safra, string Cidade,
    string? Foto) : IRequest<ProdutoDto>;
