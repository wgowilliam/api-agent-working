using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class NotificacaoRepository(AppDbContext context)
    : Repository<Notificacao>(context), INotificacaoRepository
{
    public async Task<List<Notificacao>> GetByUsuarioAsync(string usuarioId, CancellationToken ct = default)
        => await Context.Notificacoes
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.Timestamp)
            .Take(50)
            .ToListAsync(ct);

    public async Task MarkAllAsReadAsync(string usuarioId, CancellationToken ct = default)
        => await Context.Notificacoes
            .Where(n => n.UsuarioId == usuarioId && !n.Lida)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.Lida, true), ct);
}
