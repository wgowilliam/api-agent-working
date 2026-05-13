using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Compras.Queries.GetCompras;
public record GetComprasQuery(string CompradorId) : IRequest<List<CompraDto>>;
