using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Lotes.Queries.GetCatalogo;
public record GetCatalogoQuery(string? Categoria) : IRequest<List<LoteDto>>;
