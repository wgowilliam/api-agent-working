using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class CompraConfiguration : IEntityTypeConfiguration<Compra>
{
    public void Configure(EntityTypeBuilder<Compra> builder)
    {
        builder.ToTable("Compras");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.ProdutorId).HasMaxLength(50).IsRequired();
        builder.Property(c => c.CompradorId).HasMaxLength(50).IsRequired();
        builder.Property(c => c.Quantidade).HasPrecision(10, 2);
        builder.Property(c => c.PrecoUnitario).HasPrecision(10, 2);
        builder.HasOne(c => c.Produto)
            .WithMany(p => p.Compras)
            .HasForeignKey(c => c.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
