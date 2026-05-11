using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class EntregaRepository(AppDbContext context)
    : Repository<EntregaStatus>(context), IEntregaRepository
{
    public async Task<EntregaStatus?> GetByPedidoAsync(Guid pedidoId, CancellationToken ct = default)
        => await Context.Entregas.FirstOrDefaultAsync(e => e.PedidoId == pedidoId, ct);
}
