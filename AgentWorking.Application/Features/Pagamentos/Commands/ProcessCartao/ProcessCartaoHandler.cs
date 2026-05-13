using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessCartao;

public class ProcessCartaoHandler : IRequestHandler<ProcessCartaoCommand, PagamentoCartaoDto>
{
    public Task<PagamentoCartaoDto> Handle(ProcessCartaoCommand request, CancellationToken ct)
    {
        var auth = $"AUTH-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        return Task.FromResult(new PagamentoCartaoDto(Aprovado: true, CodigoAutorizacao: auth));
    }
}
