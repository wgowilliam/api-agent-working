using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class LoteRepository(AppDbContext context)
    : Repository<LoteEstoque>(context), ILoteRepository
{
    public async Task<List<LoteEstoque>> GetByCompradorAsync(string compradorId, CancellationToken ct = default)
        => await Context.Lotes
            .Include(l => l.Produto)
            .Where(l => l.CompradorId == compradorId)
            .ToListAsync(ct);

    public async Task<List<LoteEstoque>> GetCatalogoAsync(string? categoria, CancellationToken ct = default)
    {
        var query = Context.Lotes
            .Include(l => l.Produto)
            .Where(l => l.Validade > DateTime.UtcNow && l.Quantidade > 0);

        if (!string.IsNullOrEmpty(categoria)
            && Enum.TryParse<Categoria>(categoria, true, out var cat))
        {
            query = query.Where(l => l.Produto.Categoria == cat);
        }

        return await query.ToListAsync(ct);
    }
}
