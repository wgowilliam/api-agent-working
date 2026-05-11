using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class EntregaStatusConfiguration : IEntityTypeConfiguration<EntregaStatus>
{
    public void Configure(EntityTypeBuilder<EntregaStatus> builder)
    {
        builder.ToTable("Entregas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Status).HasConversion<string>();
        builder.HasOne(e => e.Pedido)
            .WithOne(p => p.Entrega)
            .HasForeignKey<EntregaStatus>(e => e.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
