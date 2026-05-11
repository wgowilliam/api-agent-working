namespace AgentWorking.Application.DTOs;
public record CompraDto(
    Guid Id, string ProdutorId, string CompradorId, Guid ProdutoId,
    string ProdutoNome, decimal Quantidade, decimal PrecoUnitario,
    DateTime DataCompra, Guid? LoteId);
