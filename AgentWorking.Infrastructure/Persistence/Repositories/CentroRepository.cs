using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class CentroRepository(AppDbContext context)
    : Repository<CentroDistribuicao>(context), ICentroRepository
{
    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await Context.Centros.AnyAsync(c => c.Id == id, ct);
}
