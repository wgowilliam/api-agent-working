using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Commands.UpdateProduto;

public class UpdateProdutoHandler(IProdutoRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateProdutoCommand, ProdutoDto>
{
    public async Task<ProdutoDto> Handle(UpdateProdutoCommand cmd, CancellationToken ct)
    {
        var produto = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Produto {cmd.Id} not found");

        produto.Nome = cmd.Nome;
        produto.Categoria = cmd.Categoria;
        produto.Quantidade = cmd.Quantidade;
        produto.Unidade = cmd.Unidade;
        produto.Preco = cmd.Preco;
        produto.Safra = cmd.Safra;
        produto.Cidade = cmd.Cidade;
        produto.Foto = cmd.Foto;

        repo.Update(produto);
        await uow.SaveChangesAsync(ct);

        return new ProdutoDto(produto.Id, produto.Nome, produto.Categoria.ToString(),
            produto.Quantidade, produto.Unidade.ToString(), produto.Preco, produto.Safra,
            produto.Cidade, produto.Foto, produto.ProdutorId, produto.Status.ToString(),
            produto.CentroDistribuicaoId);
    }
}
