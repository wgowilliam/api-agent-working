using AgentWorking.Domain.Enums;

namespace AgentWorking.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; }
    public TipoPedido Tipo { get; set; }
    public string ClienteId { get; set; } = string.Empty;
    public string CompradorId { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public StatusPedido Status { get; set; } = StatusPedido.Pendente;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataEntrega { get; set; }
    public MetodoPagamento? MetodoPagamento { get; set; }
    public string StatusPagamento { get; set; } = "pendente";
    public ICollection<ItemPedido> Itens { get; set; } = [];
    public EntregaStatus? Entrega { get; set; }
    public ICollection<Venda> Vendas { get; set; } = [];
}
