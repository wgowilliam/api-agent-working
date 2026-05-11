using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nome).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Cidade).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Safra).HasMaxLength(20).IsRequired();
        builder.Property(p => p.ProdutorId).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Foto).HasMaxLength(500);
        builder.Property(p => p.Preco).HasPrecision(10, 2);
        builder.Property(p => p.Quantidade).HasPrecision(10, 2);
        builder.Property(p => p.Categoria).HasConversion<string>();
        builder.Property(p => p.Unidade).HasConversion<string>();
        builder.Property(p => p.Status).HasConversion<string>()
            .HasDefaultValue(StatusOferta.Ativo);
        builder.HasOne(p => p.CentroDistribuicao)
            .WithMany(c => c.Produtos)
            .HasForeignKey(p => p.CentroDistribuicaoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
