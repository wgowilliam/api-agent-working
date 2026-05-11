namespace AgentWorking.Application.DTOs;
public record VendaDto(
    Guid Id, Guid? CompraId, Guid PedidoId, string ProdutorId,
    string CompradorId, decimal Quantidade, decimal ValorTotal,
    DateTime DataVenda);
