using AgentWorking.Domain.Enums;

namespace AgentWorking.Domain.Entities;

public class EntregaStatus
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public StatusEntrega Status { get; set; }
    public DateTime? TimestampSaiu { get; set; }
    public DateTime? TimestampTransporte { get; set; }
    public DateTime? TimestampEntregue { get; set; }
    public Pedido Pedido { get; set; } = null!;
}
