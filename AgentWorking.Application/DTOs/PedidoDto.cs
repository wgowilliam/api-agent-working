namespace AgentWorking.Application.DTOs;
public record ItemPedidoDto(Guid Id, Guid LoteId, string Nome, decimal Quantidade, decimal Preco);
public record PedidoDto(
    Guid Id, string Tipo, string ClienteId, string CompradorId,
    string Endereco, string Status, DateTime DataCriacao,
    DateTime? DataEntrega, string? MetodoPagamento, string StatusPagamento,
    List<ItemPedidoDto> Itens,
    string? Especie, decimal? QuantidadeTotal, DateTime? DataLimite,
    string? Observacoes, DateTime? PrazoAceite);
