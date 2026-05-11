using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Centros.Queries.GetCentros;

public class GetCentrosHandler(ICentroRepository repo) : IRequestHandler<GetCentrosQuery, List<CentroDto>>
{
    public async Task<List<CentroDto>> Handle(GetCentrosQuery request, CancellationToken ct)
    {
        var centros = await repo.GetAllAsync(ct);
        return centros.Select(c => new CentroDto(c.Id, c.Nome, c.Endereco, c.Cidade)).ToList();
    }
}
