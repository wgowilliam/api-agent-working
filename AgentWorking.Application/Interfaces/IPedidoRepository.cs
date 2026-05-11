using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface IPedidoRepository : IRepository<Pedido>
{
    Task<List<Pedido>> GetByCompradorAsync(string compradorId, CancellationToken ct = default);
    Task<List<Pedido>> GetByClienteAsync(string clienteId, CancellationToken ct = default);
    Task<List<PedidoPersonalizado>> GetExpiredPersonalizadosAsync(CancellationToken ct = default);
}
