namespace AgentWorking.Domain.Entities;

public class ItemPedido
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public Guid LoteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal Preco { get; set; }
    public Pedido Pedido { get; set; } = null!;
    public LoteEstoque Lote { get; set; } = null!;
}
