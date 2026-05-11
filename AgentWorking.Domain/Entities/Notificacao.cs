using AgentWorking.Domain.Enums;

namespace AgentWorking.Domain.Entities;

public class Notificacao
{
    public Guid Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public TipoNotificacao Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public bool Lida { get; set; }
    public DateTime Timestamp { get; set; }
}
