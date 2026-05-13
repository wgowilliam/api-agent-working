using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Entregas.Commands.PatchEntregaStatus;

public class PatchEntregaStatusHandler(
    IEntregaRepository repo,
    IPedidoRepository pedidoRepo,
    INotificacaoRepository notifRepo,
    IUnitOfWork uow) : IRequestHandler<PatchEntregaStatusCommand, EntregaDto>
{
    public async Task<EntregaDto> Handle(PatchEntregaStatusCommand cmd, CancellationToken ct)
    {
        var entrega = await repo.GetByPedidoAsync(cmd.PedidoId, ct)
            ?? throw new KeyNotFoundException($"Entrega for pedido {cmd.PedidoId} not found");

        var agora = DateTime.UtcNow;
        entrega.Status = cmd.NovoStatus;
        switch (cmd.NovoStatus)
        {
            case StatusEntrega.Saiu: entrega.TimestampSaiu = agora; break;
            case StatusEntrega.EmTransporte: entrega.TimestampTransporte = agora; break;
            case StatusEntrega.Entregue:
                entrega.TimestampEntregue = agora;
                var pedido = await pedidoRepo.GetByIdAsync(cmd.PedidoId, ct);
                if (pedido != null)
                {
                    pedido.Status = StatusPedido.Entregue;
                    pedidoRepo.Update(pedido);
                    await notifRepo.AddAsync(new Notificacao
                    {
                        Id = Guid.NewGuid(), UsuarioId = pedido.ClienteId,
                        Tipo = TipoNotificacao.Entrega, Titulo = "Pedido entregue",
                        Mensagem = "Seu pedido foi entregue com sucesso",
                        Lida = false, Timestamp = agora
                    }, ct);
                }
                break;
        }
        repo.Update(entrega);
        await uow.SaveChangesAsync(ct);

        return new EntregaDto(entrega.Id, entrega.PedidoId, entrega.Status.ToString(),
            entrega.TimestampSaiu, entrega.TimestampTransporte, entrega.TimestampEntregue);
    }
}
