using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Lotes.Commands.UpdateLote;

public class UpdateLoteHandler(ILoteRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateLoteCommand, LoteDto>
{
    public async Task<LoteDto> Handle(UpdateLoteCommand cmd, CancellationToken ct)
    {
        var lote = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Lote {cmd.Id} not found");

        lote.Quantidade = cmd.Quantidade;
        lote.PrecoVenda = cmd.PrecoVenda;
        lote.Validade = cmd.Validade;
        repo.Update(lote);
        await uow.SaveChangesAsync(ct);

        return new LoteDto(lote.Id, lote.CompraId, lote.ProdutoId, lote.Nome,
            lote.Quantidade, lote.Validade, lote.PrecoVenda, lote.CompradorId);
    }
}
