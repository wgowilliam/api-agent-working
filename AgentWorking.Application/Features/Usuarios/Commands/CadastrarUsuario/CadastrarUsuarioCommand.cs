using AgentWorking.Application.DTOs;
using AgentWorking.Domain.Enums;
using MediatR;

namespace AgentWorking.Application.Features.Usuarios.Commands.CadastrarUsuario;

public record CadastrarUsuarioCommand(string Nome, string Email, TipoUsuario Tipo)
    : IRequest<UserDto>;
