using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface IEntregaRepository : IRepository<EntregaStatus>
{
    Task<EntregaStatus?> GetByPedidoAsync(Guid pedidoId, CancellationToken ct = default);
}
