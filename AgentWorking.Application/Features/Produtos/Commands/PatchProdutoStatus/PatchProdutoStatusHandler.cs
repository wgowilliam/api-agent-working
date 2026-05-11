using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Commands.PatchProdutoStatus;

public class PatchProdutoStatusHandler(IProdutoRepository repo, IUnitOfWork uow)
    : IRequestHandler<PatchProdutoStatusCommand, ProdutoDto>
{
    public async Task<ProdutoDto> Handle(PatchProdutoStatusCommand cmd, CancellationToken ct)
    {
        var produto = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Produto {cmd.Id} not found");

        produto.Status = produto.Status switch
        {
            StatusOferta.Ativo => StatusOferta.Pausado,
            StatusOferta.Pausado => StatusOferta.Ativo,
            _ => produto.Status
        };

        repo.Update(produto);
        await uow.SaveChangesAsync(ct);

        return new ProdutoDto(produto.Id, produto.Nome, produto.Categoria.ToString(),
            produto.Quantidade, produto.Unidade.ToString(), produto.Preco, produto.Safra,
            produto.Cidade, produto.Foto, produto.ProdutorId, produto.Status.ToString(),
            produto.CentroDistribuicaoId);
    }
}
