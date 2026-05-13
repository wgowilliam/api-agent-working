using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface IProdutoRepository : IRepository<Produto>
{
    Task<List<Produto>> GetByCentroAsync(Guid centroId, CancellationToken ct = default);
    Task<List<Produto>> GetByProdutorAsync(string produtorId, CancellationToken ct = default);
    Task<List<Produto>> GetCatalogoAsync(string? categoria, string? cidade, Guid? centroId, CancellationToken ct = default);
}
