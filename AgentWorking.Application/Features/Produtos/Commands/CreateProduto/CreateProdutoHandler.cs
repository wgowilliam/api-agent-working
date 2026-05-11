using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Commands.CreateProduto;

public class CreateProdutoHandler(
    IProdutoRepository repo,
    ICentroRepository centroRepo,
    IUnitOfWork uow) : IRequestHandler<CreateProdutoCommand, ProdutoDto>
{
    public async Task<ProdutoDto> Handle(CreateProdutoCommand cmd, CancellationToken ct)
    {
        if (!await centroRepo.ExistsAsync(cmd.CentroDistribuicaoId, ct))
            throw new KeyNotFoundException($"Centro {cmd.CentroDistribuicaoId} not found");

        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Nome = cmd.Nome,
            Categoria = cmd.Categoria,
            Quantidade = cmd.Quantidade,
            Unidade = cmd.Unidade,
            Preco = cmd.Preco,
            Safra = cmd.Safra,
            Cidade = cmd.Cidade,
            ProdutorId = cmd.ProdutorId,
            CentroDistribuicaoId = cmd.CentroDistribuicaoId,
            Foto = cmd.Foto,
            Status = Domain.Enums.StatusOferta.Ativo
        };

        await repo.AddAsync(produto, ct);
        await uow.SaveChangesAsync(ct);

        return new ProdutoDto(produto.Id, produto.Nome, produto.Categoria.ToString(),
            produto.Quantidade, produto.Unidade.ToString(), produto.Preco,
            produto.Safra, produto.Cidade, produto.Foto, produto.ProdutorId,
            produto.Status.ToString(), produto.CentroDistribuicaoId);
    }
}
