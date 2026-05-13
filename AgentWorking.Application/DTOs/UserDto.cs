namespace AgentWorking.Application.DTOs;
public record UserDto(Guid Id, string Nome, string Email, string Tipo, DateTime DataCadastro);
