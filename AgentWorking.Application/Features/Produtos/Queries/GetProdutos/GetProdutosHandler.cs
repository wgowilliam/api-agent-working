using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Queries.GetProdutos;

public class GetProdutosHandler(IProdutoRepository repo) : IRequestHandler<GetProdutosQuery, List<ProdutoDto>>
{
    public async Task<List<ProdutoDto>> Handle(GetProdutosQuery request, CancellationToken ct)
    {
        List<Produto> produtos;
        if (request.CentroId.HasValue)
            produtos = await repo.GetByCentroAsync(request.CentroId.Value, ct);
        else if (!string.IsNullOrEmpty(request.ProdutorId))
            produtos = await repo.GetByProdutorAsync(request.ProdutorId, ct);
        else
            produtos = await repo.GetAllAsync(ct);

        return produtos.Select(ToDto).ToList();
    }

    private static ProdutoDto ToDto(Produto p) => new(
        p.Id, p.Nome, p.Categoria.ToString(), p.Quantidade, p.Unidade.ToString(),
        p.Preco, p.Safra, p.Cidade, p.Foto, p.ProdutorId, p.Status.ToString(),
        p.CentroDistribuicaoId);
}
