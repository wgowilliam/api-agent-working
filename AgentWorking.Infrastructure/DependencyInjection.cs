using AgentWorking.Application.Interfaces;
using AgentWorking.Infrastructure.BackgroundJobs;
using AgentWorking.Infrastructure.Persistence;
using AgentWorking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgentWorking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICentroRepository, CentroRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<ILoteRepository, LoteRepository>();
        services.AddScoped<ICompraRepository, CompraRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IVendaRepository, VendaRepository>();
        services.AddScoped<IEntregaRepository, EntregaRepository>();
        services.AddScoped<INotificacaoRepository, NotificacaoRepository>();
        services.AddHostedService<PedidoExpirationJob>();

        return services;
    }
}
