using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class TokenRevogadoRepository(AppDbContext context) : ITokenRevogadoRepository
{
    public async Task AddAsync(TokenRevogado token, CancellationToken ct = default)
        => await context.TokensRevogados.AddAsync(token, ct);

    public async Task<bool> IsRevogadoAsync(string jti, CancellationToken ct = default)
        => await context.TokensRevogados.AnyAsync(t => t.Jti == jti, ct);

    public async Task DeleteExpiredAsync(CancellationToken ct = default)
        => await context.TokensRevogados
            .Where(t => t.Expiry <= DateTime.UtcNow)
            .ExecuteDeleteAsync(ct);
}
