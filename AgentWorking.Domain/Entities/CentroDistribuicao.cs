namespace AgentWorking.Domain.Entities;

public class CentroDistribuicao
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public ICollection<Produto> Produtos { get; set; } = [];
}
