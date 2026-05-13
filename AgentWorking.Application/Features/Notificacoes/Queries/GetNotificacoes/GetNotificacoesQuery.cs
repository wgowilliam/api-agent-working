using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Notificacoes.Queries.GetNotificacoes;
public record GetNotificacoesQuery(string UsuarioId) : IRequest<List<NotificacaoDto>>;
