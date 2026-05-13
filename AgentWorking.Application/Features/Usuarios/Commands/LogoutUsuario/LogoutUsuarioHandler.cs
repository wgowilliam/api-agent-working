using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using MediatR;

namespace AgentWorking.Application.Features.Usuarios.Commands.LogoutUsuario;

public class LogoutUsuarioHandler(
    ITokenRevogadoRepository repo,
    IUnitOfWork uow) : IRequestHandler<LogoutUsuarioCommand>
{
    public async Task Handle(LogoutUsuarioCommand cmd, CancellationToken ct)
    {
        await repo.AddAsync(new TokenRevogado
        {
            Id = Guid.NewGuid(),
            Jti = cmd.Jti,
            Expiry = cmd.Expiry
        }, ct);
        await uow.SaveChangesAsync(ct);
    }
}
