using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("PedidoItens");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Nome).HasMaxLength(200).IsRequired();
        builder.Property(i => i.Quantidade).HasPrecision(10, 2);
        builder.Property(i => i.Preco).HasPrecision(10, 2);
        builder.HasOne(i => i.Pedido)
            .WithMany(p => p.Itens)
            .HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(i => i.Lote)
            .WithMany(l => l.ItensPedido)
            .HasForeignKey(i => i.LoteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
