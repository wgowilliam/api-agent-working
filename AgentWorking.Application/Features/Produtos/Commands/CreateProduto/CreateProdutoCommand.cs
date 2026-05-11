using AgentWorking.Application.DTOs;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Commands.CreateProduto;
public record CreateProdutoCommand(
    string Nome, Categoria Categoria, decimal Quantidade, UnidadeMedida Unidade,
    decimal Preco, string Safra, string Cidade, string ProdutorId,
    Guid CentroDistribuicaoId, string? Foto) : IRequest<ProdutoDto>;
