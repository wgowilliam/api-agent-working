namespace AgentWorking.Application.DTOs;
public record PagamentoPixDto(string QrCode, string ChavePix, decimal Valor);
public record PagamentoCartaoDto(bool Aprovado, string CodigoAutorizacao);
public record PagamentoBoletoDto(string CodigoBarras, string LinhaDigitavel, string PdfUrl);
