using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
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

    public async Task<List<Produto>> GetCatalogoAsync(
        string? categoria, string? cidade, Guid? centroId, CancellationToken ct = default)
    {
        var query = Context.Produtos
            .Include(p => p.CentroDistribuicao)
            .Where(p => p.Status == StatusOferta.Ativo && p.Quantidade > 0);

        if (!string.IsNullOrEmpty(categoria) && Enum.TryParse<Categoria>(categoria, true, out var cat))
            query = query.Where(p => p.Categoria == cat);

        if (!string.IsNullOrEmpty(cidade))
            query = query.Where(p => EF.Functions.ILike(p.Cidade, $"%{cidade}%"));

        if (centroId.HasValue)
            query = query.Where(p => p.CentroDistribuicaoId == centroId.Value);

        return await query.OrderBy(p => p.Nome).ToListAsync(ct);
    }
}
