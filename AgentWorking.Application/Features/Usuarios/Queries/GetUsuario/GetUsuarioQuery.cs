using AgentWorking.Application.DTOs;
using MediatR;

namespace AgentWorking.Application.Features.Usuarios.Queries.GetUsuario;

public record GetUsuarioQuery(Guid Id) : IRequest<UserDto>;
