using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Centros.Queries.GetCentros;
public record GetCentrosQuery : IRequest<List<CentroDto>>;
