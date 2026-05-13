using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessBoleto;

public class ProcessBoletoHandler : IRequestHandler<ProcessBoletoCommand, PagamentoBoletoDto>
{
    public Task<PagamentoBoletoDto> Handle(ProcessBoletoCommand request, CancellationToken ct)
    {
        var codigo = $"34191.09008 61234.678901 23456.789012 3 {(long)(request.Valor * 100):D17}";
        return Task.FromResult(new PagamentoBoletoDto(
            CodigoBarras: codigo.Replace(" ", ""),
            LinhaDigitavel: codigo,
            PdfUrl: $"/api/pagamentos/boleto/{request.PedidoId}/pdf"));
    }
}
