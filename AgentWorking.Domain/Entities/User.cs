using AgentWorking.Domain.Enums;

namespace AgentWorking.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public TipoUsuario Tipo { get; set; }
    public DateTime DataCadastro { get; set; }
}
