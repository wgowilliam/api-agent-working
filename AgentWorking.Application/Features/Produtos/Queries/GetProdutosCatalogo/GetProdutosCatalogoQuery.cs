using AgentWorking.Application.DTOs;
using MediatR;

namespace AgentWorking.Application.Features.Produtos.Queries.GetProdutosCatalogo;

public record GetProdutosCatalogoQuery(
    string? Categoria,
    string? Cidade,
    Guid? CentroId) : IRequest<List<ProdutoCatalogoDto>>;
