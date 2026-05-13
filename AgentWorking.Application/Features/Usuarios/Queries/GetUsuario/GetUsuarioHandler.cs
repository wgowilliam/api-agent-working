using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;

namespace AgentWorking.Application.Features.Usuarios.Queries.GetUsuario;

public class GetUsuarioHandler(IUsuarioRepository repo)
    : IRequestHandler<GetUsuarioQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUsuarioQuery request, CancellationToken ct)
    {
        var user = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException($"Usuário {request.Id} não encontrado.");

        return new UserDto(user.Id, user.Nome, user.Email, user.Tipo.ToString(), user.DataCadastro);
    }
}
