using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface ILoteRepository : IRepository<LoteEstoque>
{
    Task<List<LoteEstoque>> GetByCompradorAsync(string compradorId, CancellationToken ct = default);
    Task<List<LoteEstoque>> GetCatalogoAsync(string? categoria, CancellationToken ct = default);
}
