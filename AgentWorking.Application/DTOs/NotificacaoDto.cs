namespace AgentWorking.Application.DTOs;
public record NotificacaoDto(
    Guid Id, string UsuarioId, string Tipo, string Titulo,
    string Mensagem, bool Lida, DateTime Timestamp);
