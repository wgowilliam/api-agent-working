using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface INotificacaoRepository : IRepository<Notificacao>
{
    Task<List<Notificacao>> GetByUsuarioAsync(string usuarioId, CancellationToken ct = default);
    Task MarkAllAsReadAsync(string usuarioId, CancellationToken ct = default);
}
