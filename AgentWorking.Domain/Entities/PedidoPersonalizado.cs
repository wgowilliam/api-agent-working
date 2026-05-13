namespace AgentWorking.Domain.Entities;

public class PedidoPersonalizado : Pedido
{
    public string? Especie { get; set; }
    public decimal? QuantidadeTotal { get; set; }
    public DateTime? DataLimite { get; set; }
    public string? Observacoes { get; set; }
    public DateTime? PrazoAceite { get; set; }
}
