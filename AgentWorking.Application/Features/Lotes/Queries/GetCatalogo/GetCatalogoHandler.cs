using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Lotes.Queries.GetCatalogo;

public class GetCatalogoHandler(ILoteRepository repo) : IRequestHandler<GetCatalogoQuery, List<LoteDto>>
{
    public async Task<List<LoteDto>> Handle(GetCatalogoQuery request, CancellationToken ct)
    {
        var lotes = await repo.GetCatalogoAsync(request.Categoria, ct);
        return lotes.Select(l => new LoteDto(
            l.Id, l.CompraId, l.ProdutoId, l.Nome,
            l.Quantidade, l.Validade, l.PrecoVenda, l.CompradorId)).ToList();
    }
}
