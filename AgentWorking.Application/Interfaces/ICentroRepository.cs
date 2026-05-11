using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface ICentroRepository : IRepository<CentroDistribuicao>
{
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
