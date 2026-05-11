using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class LoteEstoqueConfiguration : IEntityTypeConfiguration<LoteEstoque>
{
    public void Configure(EntityTypeBuilder<LoteEstoque> builder)
    {
        builder.ToTable("Lotes");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Nome).HasMaxLength(200).IsRequired();
        builder.Property(l => l.CompradorId).HasMaxLength(50).IsRequired();
        builder.Property(l => l.Quantidade).HasPrecision(10, 2);
        builder.Property(l => l.PrecoVenda).HasPrecision(10, 2);
        builder.Property(l => l.Validade).IsRequired();
        builder.HasOne(l => l.Compra)
            .WithOne(c => c.Lote)
            .HasForeignKey<LoteEstoque>(l => l.CompraId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.Produto)
            .WithMany()
            .HasForeignKey(l => l.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
