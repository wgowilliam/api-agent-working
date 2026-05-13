using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using MediatR;

namespace AgentWorking.Application.Features.Produtos.Queries.GetProdutosCatalogo;

public class GetProdutosCatalogoHandler(IProdutoRepository repo)
    : IRequestHandler<GetProdutosCatalogoQuery, List<ProdutoCatalogoDto>>
{
    public async Task<List<ProdutoCatalogoDto>> Handle(GetProdutosCatalogoQuery request, CancellationToken ct)
    {
        var produtos = await repo.GetCatalogoAsync(request.Categoria, request.Cidade, request.CentroId, ct);
        return produtos.Select(ToDto).ToList();
    }

    private static ProdutoCatalogoDto ToDto(Produto p) => new(
        p.Id, p.Nome, p.Categoria.ToString(), p.Quantidade, p.Unidade.ToString(),
        p.Preco, p.Safra, p.Cidade, p.Foto, p.ProdutorId,
        p.CentroDistribuicaoId, p.CentroDistribuicao.Nome, p.CentroDistribuicao.Cidade);
}
