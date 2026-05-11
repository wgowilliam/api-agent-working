namespace AgentWorking.Domain.Entities;

public class Compra
{
    public Guid Id { get; set; }
    public string ProdutorId { get; set; } = string.Empty;
    public string CompradorId { get; set; } = string.Empty;
    public Guid ProdutoId { get; set; }
    public decimal Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public DateTime DataCompra { get; set; }
    public Produto Produto { get; set; } = null!;
    public LoteEstoque? Lote { get; set; }
}
