using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class VendaRepository(AppDbContext context)
    : Repository<Venda>(context), IVendaRepository
{
    public async Task<List<Venda>> GetByProdutorAsync(string produtorId, CancellationToken ct = default)
        => await Context.Vendas
            .Include(v => v.Pedido)
            .Where(v => v.ProdutorId == produtorId)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync(ct);
}
