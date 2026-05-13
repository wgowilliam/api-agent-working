using AgentWorking.Application.Features.Pedidos.Commands.ExpirePedidos;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AgentWorking.Infrastructure.BackgroundJobs;

public class PedidoExpirationJob(
    IServiceScopeFactory scopeFactory,
    ILogger<PedidoExpirationJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new ExpirePedidosCommand(), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in PedidoExpirationJob");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
