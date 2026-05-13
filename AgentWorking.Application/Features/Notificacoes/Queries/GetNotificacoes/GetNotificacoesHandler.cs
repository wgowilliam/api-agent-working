using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Notificacoes.Queries.GetNotificacoes;

public class GetNotificacoesHandler(INotificacaoRepository repo)
    : IRequestHandler<GetNotificacoesQuery, List<NotificacaoDto>>
{
    public async Task<List<NotificacaoDto>> Handle(GetNotificacoesQuery request, CancellationToken ct)
    {
        var notifs = await repo.GetByUsuarioAsync(request.UsuarioId, ct);
        return notifs.Select(n => new NotificacaoDto(
            n.Id, n.UsuarioId, n.Tipo.ToString(), n.Titulo, n.Mensagem, n.Lida, n.Timestamp)).ToList();
    }
}
