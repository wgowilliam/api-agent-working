using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.ProdutorId).HasMaxLength(50).IsRequired();
        builder.Property(v => v.CompradorId).HasMaxLength(50).IsRequired();
        builder.Property(v => v.Quantidade).HasPrecision(10, 2);
        builder.Property(v => v.ValorTotal).HasPrecision(10, 2);
        builder.HasOne(v => v.Pedido)
            .WithMany(p => p.Vendas)
            .HasForeignKey(v => v.PedidoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
