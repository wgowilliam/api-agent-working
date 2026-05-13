using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.ClienteId).HasMaxLength(50).IsRequired();
        builder.Property(p => p.CompradorId).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Endereco).HasMaxLength(500).IsRequired();
        builder.Property(p => p.StatusPagamento).HasMaxLength(20);
        builder.Property(p => p.Status).HasConversion<string>();
        builder.Property(p => p.MetodoPagamento).HasConversion<string>();

        builder.HasDiscriminator(p => p.Tipo)
            .HasValue<Pedido>(TipoPedido.Padrao)
            .HasValue<PedidoPersonalizado>(TipoPedido.Personalizado);
        builder.Property(p => p.Tipo).HasConversion<string>();
    }
}
