namespace AgentWorking.Domain.Entities;

public class LoteEstoque
{
    public Guid Id { get; set; }
    public Guid CompraId { get; set; }
    public Guid ProdutoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public DateTime Validade { get; set; }
    public decimal PrecoVenda { get; set; }
    public string CompradorId { get; set; } = string.Empty;
    public Compra Compra { get; set; } = null!;
    public Produto Produto { get; set; } = null!;
    public ICollection<ItemPedido> ItensPedido { get; set; } = [];
}
