using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class UsuarioRepository(AppDbContext context)
    : Repository<User>(context), IUsuarioRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await Context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
}
