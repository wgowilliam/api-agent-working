namespace AgentWorking.Application.DTOs;
public record LoteDto(
    Guid Id, Guid CompraId, Guid ProdutoId, string Nome,
    decimal Quantidade, DateTime Validade, decimal PrecoVenda,
    string CompradorId);
