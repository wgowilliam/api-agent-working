using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Compras.Commands.CreateCompra;

public class CreateCompraHandler(
    IProdutoRepository produtoRepo,
    ICompraRepository compraRepo,
    ILoteRepository loteRepo,
    INotificacaoRepository notifRepo,
    IUnitOfWork uow) : IRequestHandler<CreateCompraCommand, CompraDto>
{
    public async Task<CompraDto> Handle(CreateCompraCommand cmd, CancellationToken ct)
    {
        var produto = await produtoRepo.GetByIdAsync(cmd.ProdutoId, ct)
            ?? throw new KeyNotFoundException($"Produto {cmd.ProdutoId} not found");

        if (produto.Quantidade < cmd.Quantidade)
            throw new InvalidOperationException(
                $"Estoque insuficiente. Disponível: {produto.Quantidade}");

        produto.Quantidade -= cmd.Quantidade;
        produtoRepo.Update(produto);

        var compra = new Compra
        {
            Id = Guid.NewGuid(),
            ProdutorId = produto.ProdutorId,
            CompradorId = cmd.CompradorId,
            ProdutoId = cmd.ProdutoId,
            Quantidade = cmd.Quantidade,
            PrecoUnitario = cmd.PrecoUnitario,
            DataCompra = DateTime.UtcNow
        };
        await compraRepo.AddAsync(compra, ct);

        var lote = new LoteEstoque
        {
            Id = Guid.NewGuid(),
            CompraId = compra.Id,
            ProdutoId = cmd.ProdutoId,
            Nome = produto.Nome,
            Quantidade = cmd.Quantidade,
            Validade = cmd.Validade,
            PrecoVenda = cmd.PrecoVenda,
            CompradorId = cmd.CompradorId
        };
        await loteRepo.AddAsync(lote, ct);

        var notif = new Notificacao
        {
            Id = Guid.NewGuid(),
            UsuarioId = produto.ProdutorId,
            Tipo = TipoNotificacao.Pedido,
            Titulo = "Produto comprado",
            Mensagem = $"Comprador {cmd.CompradorId} comprou {cmd.Quantidade} de {produto.Nome}",
            Lida = false,
            Timestamp = DateTime.UtcNow
        };
        await notifRepo.AddAsync(notif, ct);

        await uow.SaveChangesAsync(ct);

        return new CompraDto(compra.Id, compra.ProdutorId, compra.CompradorId,
            compra.ProdutoId, produto.Nome, compra.Quantidade,
            compra.PrecoUnitario, compra.DataCompra, lote.Id);
    }
}
