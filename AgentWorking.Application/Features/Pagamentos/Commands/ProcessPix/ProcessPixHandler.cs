using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessPix;

public class ProcessPixHandler : IRequestHandler<ProcessPixCommand, PagamentoPixDto>
{
    public Task<PagamentoPixDto> Handle(ProcessPixCommand request, CancellationToken ct)
    {
        const string mockQr = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
        return Task.FromResult(new PagamentoPixDto(
            QrCode: mockQr,
            ChavePix: "pagamentos@portalagro.com.br",
            Valor: request.Valor));
    }
}
