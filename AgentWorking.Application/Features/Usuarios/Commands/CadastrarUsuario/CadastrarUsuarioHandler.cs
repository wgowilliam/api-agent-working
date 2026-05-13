using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using MediatR;

namespace AgentWorking.Application.Features.Usuarios.Commands.CadastrarUsuario;

public class CadastrarUsuarioHandler(
    IUsuarioRepository repo,
    IUnitOfWork uow) : IRequestHandler<CadastrarUsuarioCommand, UserDto>
{
    public async Task<UserDto> Handle(CadastrarUsuarioCommand cmd, CancellationToken ct)
    {
        var existing = await repo.GetByEmailAsync(cmd.Email, ct);
        if (existing is not null)
            throw new InvalidOperationException($"Email '{cmd.Email}' já está cadastrado.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Nome = cmd.Nome,
            Email = cmd.Email,
            Tipo = cmd.Tipo,
            DataCadastro = DateTime.UtcNow,
        };

        await repo.AddAsync(user, ct);
        await uow.SaveChangesAsync(ct);

        return new UserDto(user.Id, user.Nome, user.Email, user.Tipo.ToString(), user.DataCadastro);
    }
}
