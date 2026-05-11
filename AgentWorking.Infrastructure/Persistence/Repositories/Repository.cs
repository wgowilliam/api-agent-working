using AgentWorking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : class
{
    protected readonly AppDbContext Context = context;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await Context.Set<T>().FindAsync([id], ct);

    public async Task<List<T>> GetAllAsync(CancellationToken ct = default)
        => await Context.Set<T>().ToListAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await Context.Set<T>().AddAsync(entity, ct);

    public void Update(T entity) => Context.Set<T>().Update(entity);

    public void Remove(T entity) => Context.Set<T>().Remove(entity);
}
