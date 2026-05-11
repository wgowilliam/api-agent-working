using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface IVendaRepository : IRepository<Venda>
{
    Task<List<Venda>> GetByProdutorAsync(string produtorId, CancellationToken ct = default);
}
