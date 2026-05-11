namespace AgentWorking.Application.DTOs;
public record EntregaDto(
    Guid Id, Guid PedidoId, string Status,
    DateTime? TimestampSaiu, DateTime? TimestampTransporte,
    DateTime? TimestampEntregue);
