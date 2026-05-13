using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.ExpirePedidos;

public class ExpirePedidosHandler(
    IPedidoRepository pedidoRepo,
    INotificacaoRepository notifRepo,
    IUnitOfWork uow) : IRequestHandler<ExpirePedidosCommand>
{
    public async Task Handle(ExpirePedidosCommand request, CancellationToken ct)
    {
        var expired = await pedidoRepo.GetExpiredPersonalizadosAsync(ct);
        if (expired.Count == 0) return;

        foreach (var pedido in expired)
        {
            pedido.Status = StatusPedido.Expirado;
            await notifRepo.AddAsync(new Notificacao
            {
                Id = Guid.NewGuid(),
                UsuarioId = pedido.ClienteId,
                Tipo = TipoNotificacao.Alerta,
                Titulo = "Pedido personalizado expirado",
                Mensagem = $"Nenhum comprador aceitou seu pedido de {pedido.Especie} no prazo",
                Lida = false,
                Timestamp = DateTime.UtcNow
            }, ct);
        }

        await uow.SaveChangesAsync(ct);
    }
}
