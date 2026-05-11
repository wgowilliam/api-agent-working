using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class CentroDistribuicaoConfiguration : IEntityTypeConfiguration<CentroDistribuicao>
{
    public void Configure(EntityTypeBuilder<CentroDistribuicao> builder)
    {
        builder.ToTable("Centros");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nome).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Endereco).HasMaxLength(500).IsRequired();
        builder.Property(c => c.Cidade).HasMaxLength(100).IsRequired();
    }
}
