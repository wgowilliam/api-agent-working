using MediatR;
namespace AgentWorking.Application.Features.Notificacoes.Commands.PatchNotificacaoLida;
public record PatchNotificacaoLidaCommand(Guid Id) : IRequest;
