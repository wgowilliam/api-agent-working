namespace AgentWorking.Domain.Entities;

public class TokenRevogado
{
    public Guid Id { get; set; }
    public string Jti { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
}
