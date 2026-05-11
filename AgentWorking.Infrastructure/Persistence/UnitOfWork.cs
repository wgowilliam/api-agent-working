using AgentWorking.Application.Interfaces;

namespace AgentWorking.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await context.SaveChangesAsync(ct);
}
