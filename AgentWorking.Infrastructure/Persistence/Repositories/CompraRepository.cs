using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class CompraRepository(AppDbContext context)
    : Repository<Compra>(context), ICompraRepository
{
    public async Task<List<Compra>> GetByCompradorAsync(string compradorId, CancellationToken ct = default)
        => await Context.Compras
            .Include(c => c.Produto)
            .Include(c => c.Lote)
            .Where(c => c.CompradorId == compradorId)
            .OrderByDescending(c => c.DataCompra)
            .ToListAsync(ct);
}
