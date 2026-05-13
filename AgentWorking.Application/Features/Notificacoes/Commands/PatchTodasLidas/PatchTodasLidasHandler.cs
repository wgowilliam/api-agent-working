using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Notificacoes.Commands.PatchTodasLidas;

public class PatchTodasLidasHandler(INotificacaoRepository repo)
    : IRequestHandler<PatchTodasLidasCommand>
{
    public async Task Handle(PatchTodasLidasCommand cmd, CancellationToken ct)
        => await repo.MarkAllAsReadAsync(cmd.UsuarioId, ct);
}
