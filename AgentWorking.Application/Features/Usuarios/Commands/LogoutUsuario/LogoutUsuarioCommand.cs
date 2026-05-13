using MediatR;

namespace AgentWorking.Application.Features.Usuarios.Commands.LogoutUsuario;

public record LogoutUsuarioCommand(string Jti, DateTime Expiry) : IRequest;
