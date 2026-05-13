using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Notificacoes.Commands.PatchNotificacaoLida;

public class PatchNotificacaoLidaHandler(INotificacaoRepository repo, IUnitOfWork uow)
    : IRequestHandler<PatchNotificacaoLidaCommand>
{
    public async Task Handle(PatchNotificacaoLidaCommand cmd, CancellationToken ct)
    {
        var notif = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Notificacao {cmd.Id} not found");
        notif.Lida = true;
        repo.Update(notif);
        await uow.SaveChangesAsync(ct);
    }
}
