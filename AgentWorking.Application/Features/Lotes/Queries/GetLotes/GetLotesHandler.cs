using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Lotes.Queries.GetLotes;

public class GetLotesHandler(ILoteRepository repo) : IRequestHandler<GetLotesQuery, List<LoteDto>>
{
    public async Task<List<LoteDto>> Handle(GetLotesQuery request, CancellationToken ct)
    {
        var lotes = await repo.GetByCompradorAsync(request.CompradorId, ct);
        return lotes.Select(l => new LoteDto(
            l.Id, l.CompraId, l.ProdutoId, l.Nome,
            l.Quantidade, l.Validade, l.PrecoVenda, l.CompradorId)).ToList();
    }
}
