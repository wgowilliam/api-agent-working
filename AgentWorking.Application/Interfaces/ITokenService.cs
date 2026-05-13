using AgentWorking.Domain.Entities;

namespace AgentWorking.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    DateTime GetExpiry();
}
