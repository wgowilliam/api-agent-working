using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;

namespace AgentWorking.Application.Interfaces;

public interface IUsuarioRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
}
