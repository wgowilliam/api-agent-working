namespace AgentWorking.Domain.Entities;

public class PedidoPersonalizado : Pedido
{
    public string Especie { get; set; } = string.Empty;
    public decimal QuantidadeTotal { get; set; }
    public DateTime DataLimite { get; set; }
    public string Observacoes { get; set; } = string.Empty;
    public DateTime PrazoAceite { get; set; }
}
