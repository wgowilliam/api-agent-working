using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class PedidoRepository(AppDbContext context)
    : Repository<Pedido>(context), IPedidoRepository
{
    public async Task<List<Pedido>> GetByCompradorAsync(string compradorId, CancellationToken ct = default)
        => await Context.Pedidos
            .Include(p => p.Itens)
            .Where(p => p.CompradorId == compradorId)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync(ct);

    public async Task<List<Pedido>> GetByClienteAsync(string clienteId, CancellationToken ct = default)
        => await Context.Pedidos
            .Include(p => p.Itens)
            .Include(p => p.Entrega)
            .Where(p => p.ClienteId == clienteId)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync(ct);

    public async Task<List<PedidoPersonalizado>> GetExpiredPersonalizadosAsync(CancellationToken ct = default)
        => await Context.PedidosPersonalizados
            .Where(p => p.Status == StatusPedido.Pendente && p.PrazoAceite < DateTime.UtcNow)
            .ToListAsync(ct);
}
