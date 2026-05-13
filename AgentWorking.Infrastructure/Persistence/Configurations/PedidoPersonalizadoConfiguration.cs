using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class PedidoPersonalizadoConfiguration : IEntityTypeConfiguration<PedidoPersonalizado>
{
    public void Configure(EntityTypeBuilder<PedidoPersonalizado> builder)
    {
        builder.Property(p => p.Especie).HasMaxLength(200);
        builder.Property(p => p.QuantidadeTotal).HasPrecision(10, 2);
        builder.Property(p => p.Observacoes).HasMaxLength(1000);
    }
}
