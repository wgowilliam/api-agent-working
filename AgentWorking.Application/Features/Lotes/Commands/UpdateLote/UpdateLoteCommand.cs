using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Lotes.Commands.UpdateLote;
public record UpdateLoteCommand(Guid Id, decimal Quantidade, decimal PrecoVenda, DateTime Validade)
    : IRequest<LoteDto>;
