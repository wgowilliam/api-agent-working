using AgentWorking.Domain.Enums;

namespace AgentWorking.Domain.Entities;

public class Produto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Categoria Categoria { get; set; }
    public decimal Quantidade { get; set; }
    public UnidadeMedida Unidade { get; set; }
    public decimal Preco { get; set; }
    public string Safra { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string? Foto { get; set; }
    public string ProdutorId { get; set; } = string.Empty;
    public StatusOferta Status { get; set; } = StatusOferta.Ativo;
    public Guid CentroDistribuicaoId { get; set; }
    public CentroDistribuicao CentroDistribuicao { get; set; } = null!;
    public ICollection<Compra> Compras { get; set; } = [];
}
