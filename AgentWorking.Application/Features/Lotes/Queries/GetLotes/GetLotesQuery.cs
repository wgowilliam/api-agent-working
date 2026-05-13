using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Lotes.Queries.GetLotes;
public record GetLotesQuery(string CompradorId) : IRequest<List<LoteDto>>;
