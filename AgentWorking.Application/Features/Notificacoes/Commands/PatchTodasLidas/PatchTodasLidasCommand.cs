using MediatR;
namespace AgentWorking.Application.Features.Notificacoes.Commands.PatchTodasLidas;
public record PatchTodasLidasCommand(string UsuarioId) : IRequest;
