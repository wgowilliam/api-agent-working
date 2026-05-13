using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.PatchPedidoStatus;

public class PatchPedidoStatusHandler(IPedidoRepository repo, INotificacaoRepository notifRepo, IUnitOfWork uow)
    : IRequestHandler<PatchPedidoStatusCommand, PedidoDto>
{
    public async Task<PedidoDto> Handle(PatchPedidoStatusCommand cmd, CancellationToken ct)
    {
        var pedido = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Pedido {cmd.Id} not found");

        pedido.Status = cmd.NovoStatus;
        if (cmd.NovaDataEntrega.HasValue) pedido.DataEntrega = cmd.NovaDataEntrega;
        repo.Update(pedido);

        await notifRepo.AddAsync(new Notificacao
        {
            Id = Guid.NewGuid(),
            UsuarioId = pedido.ClienteId,
            Tipo = TipoNotificacao.Pedido,
            Titulo = $"Pedido {cmd.NovoStatus}",
            Mensagem = $"Seu pedido foi {cmd.NovoStatus.ToString().ToLower()}",
            Lida = false, Timestamp = DateTime.UtcNow
        }, ct);

        await uow.SaveChangesAsync(ct);

        var pp = pedido as PedidoPersonalizado;
        return new PedidoDto(pedido.Id, pedido.Tipo.ToString(), pedido.ClienteId,
            pedido.CompradorId, pedido.Endereco, pedido.Status.ToString(),
            pedido.DataCriacao, pedido.DataEntrega, pedido.MetodoPagamento?.ToString(),
            pedido.StatusPagamento,
            pedido.Itens.Select(i => new ItemPedidoDto(i.Id, i.LoteId, i.Nome, i.Quantidade, i.Preco)).ToList(),
            pp?.Especie, pp?.QuantidadeTotal, pp?.DataLimite, pp?.Observacoes, pp?.PrazoAceite);
    }
}
