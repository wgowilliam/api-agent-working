using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface ICompraRepository : IRepository<Compra>
{
    Task<List<Compra>> GetByCompradorAsync(string compradorId, CancellationToken ct = default);
}
