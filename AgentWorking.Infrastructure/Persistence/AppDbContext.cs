using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CentroDistribuicao> Centros => Set<CentroDistribuicao>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<LoteEstoque> Lotes => Set<LoteEstoque>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoPersonalizado> PedidosPersonalizados => Set<PedidoPersonalizado>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<EntregaStatus> Entregas => Set<EntregaStatus>();
    public DbSet<Notificacao> Notificacoes => Set<Notificacao>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
