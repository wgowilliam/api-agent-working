using AgentWorking.Application.DTOs;
using MediatR;

namespace AgentWorking.Application.Features.Usuarios.Commands.LoginUsuario;

public record LoginUsuarioCommand(string Email, string Senha) : IRequest<TokenDto>;
