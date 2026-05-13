using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Vendas.Queries.GetVendas;
public record GetVendasQuery(string ProdutorId) : IRequest<List<VendaDto>>;
