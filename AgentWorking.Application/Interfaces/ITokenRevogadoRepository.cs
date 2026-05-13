namespace AgentWorking.Application.Interfaces;

public interface ITokenRevogadoRepository
{
    Task AddAsync(Domain.Entities.TokenRevogado token, CancellationToken ct = default);
    Task<bool> IsRevogadoAsync(string jti, CancellationToken ct = default);
    Task DeleteExpiredAsync(CancellationToken ct = default);
}
