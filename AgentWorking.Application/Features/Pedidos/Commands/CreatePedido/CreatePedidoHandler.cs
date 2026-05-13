using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.CreatePedido;

public class CreatePedidoHandler(
    IPedidoRepository pedidoRepo,
    ILoteRepository loteRepo,
    IVendaRepository vendaRepo,
    IEntregaRepository entregaRepo,
    INotificacaoRepository notifRepo,
    ICompraRepository compraRepo,
    IUnitOfWork uow) : IRequestHandler<CreatePedidoCommand, PedidoDto>
{
    public async Task<PedidoDto> Handle(CreatePedidoCommand cmd, CancellationToken ct)
    {
        var lotes = new Dictionary<Guid, LoteEstoque>();
        foreach (var item in cmd.Itens)
        {
            var lote = await loteRepo.GetByIdAsync(item.LoteId, ct)
                ?? throw new KeyNotFoundException($"Lote {item.LoteId} not found");
            if (lote.Quantidade < item.Quantidade)
                throw new InvalidOperationException(
                    $"Lote {lote.Nome}: estoque insuficiente. Disponível: {lote.Quantidade}");
            lotes[item.LoteId] = lote;
        }

        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            Tipo = TipoPedido.Padrao,
            ClienteId = cmd.ClienteId,
            CompradorId = cmd.CompradorId,
            Endereco = cmd.Endereco,
            Status = StatusPedido.Confirmado,
            DataCriacao = DateTime.UtcNow,
            MetodoPagamento = cmd.MetodoPagamento,
            StatusPagamento = "pendente"
        };
        await pedidoRepo.AddAsync(pedido, ct);

        foreach (var item in cmd.Itens)
        {
            var lote = lotes[item.LoteId];
            lote.Quantidade -= item.Quantidade;
            loteRepo.Update(lote);

            pedido.Itens.Add(new ItemPedido
            {
                Id = Guid.NewGuid(),
                PedidoId = pedido.Id,
                LoteId = item.LoteId,
                Nome = item.Nome,
                Quantidade = item.Quantidade,
                Preco = item.Preco
            });

            var compra = await compraRepo.GetByIdAsync(lote.CompraId, ct);
            if (compra != null)
            {
                await vendaRepo.AddAsync(new Venda
                {
                    Id = Guid.NewGuid(),
                    CompraId = lote.CompraId,
                    PedidoId = pedido.Id,
                    ProdutorId = compra.ProdutorId,
                    CompradorId = cmd.CompradorId,
                    Quantidade = item.Quantidade,
                    ValorTotal = item.Quantidade * item.Preco,
                    DataVenda = DateTime.UtcNow
                }, ct);

                await notifRepo.AddAsync(new Notificacao
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = compra.ProdutorId,
                    Tipo = TipoNotificacao.Pedido,
                    Titulo = "Produto vendido",
                    Mensagem = $"{item.Quantidade} {item.Nome} vendido ao cliente",
                    Lida = false, Timestamp = DateTime.UtcNow
                }, ct);
            }
        }

        await entregaRepo.AddAsync(new EntregaStatus
        {
            Id = Guid.NewGuid(), PedidoId = pedido.Id, Status = StatusEntrega.Saiu
        }, ct);

        await notifRepo.AddAsync(new Notificacao
        {
            Id = Guid.NewGuid(),
            UsuarioId = cmd.CompradorId,
            Tipo = TipoNotificacao.Pedido,
            Titulo = "Novo pedido recebido",
            Mensagem = $"Pedido de {cmd.ClienteId} confirmado",
            Lida = false, Timestamp = DateTime.UtcNow
        }, ct);

        await uow.SaveChangesAsync(ct);

        return new PedidoDto(pedido.Id, pedido.Tipo.ToString(), pedido.ClienteId,
            pedido.CompradorId, pedido.Endereco, pedido.Status.ToString(),
            pedido.DataCriacao, pedido.DataEntrega,
            pedido.MetodoPagamento?.ToString(), pedido.StatusPagamento,
            pedido.Itens.Select(i => new ItemPedidoDto(i.Id, i.LoteId, i.Nome, i.Quantidade, i.Preco)).ToList(),
            null, null, null, null, null);
    }
}
