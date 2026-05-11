using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Queries.GetProdutos;
public record GetProdutosQuery(Guid? CentroId, string? ProdutorId) : IRequest<List<ProdutoDto>>;
