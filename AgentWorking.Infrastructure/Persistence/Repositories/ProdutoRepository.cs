using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class ProdutoRepository(AppDbContext context)
    : Repository<Produto>(context), IProdutoRepository
{
    public async Task<List<Produto>> GetByCentroAsync(Guid centroId, CancellationToken ct = default)
        => await Context.Produtos
            .Where(p => p.CentroDistribuicaoId == centroId)
            .ToListAsync(ct);

    public async Task<List<Produto>> GetByProdutorAsync(string produtorId, CancellationToken ct = default)
        => await Context.Produtos
            .Where(p => p.ProdutorId == produtorId)
            .ToListAsync(ct);
}
