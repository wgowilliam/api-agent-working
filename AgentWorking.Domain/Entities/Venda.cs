namespace AgentWorking.Domain.Entities;

public class Venda
{
    public Guid Id { get; set; }
    public Guid? CompraId { get; set; }
    public Guid PedidoId { get; set; }
    public string ProdutorId { get; set; } = string.Empty;
    public string CompradorId { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataVenda { get; set; }
    public Pedido Pedido { get; set; } = null!;
}
