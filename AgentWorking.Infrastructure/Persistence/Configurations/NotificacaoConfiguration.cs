using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class NotificacaoConfiguration : IEntityTypeConfiguration<Notificacao>
{
    public void Configure(EntityTypeBuilder<Notificacao> builder)
    {
        builder.ToTable("Notificacoes");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.UsuarioId).HasMaxLength(50).IsRequired();
        builder.Property(n => n.Titulo).HasMaxLength(200).IsRequired();
        builder.Property(n => n.Mensagem).HasMaxLength(1000).IsRequired();
        builder.Property(n => n.Tipo).HasConversion<string>();
        builder.HasIndex(n => n.UsuarioId);
        builder.HasIndex(n => new { n.UsuarioId, n.Lida });
    }
}
