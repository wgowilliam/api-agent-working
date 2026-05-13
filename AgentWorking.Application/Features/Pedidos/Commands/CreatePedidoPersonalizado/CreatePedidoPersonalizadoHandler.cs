using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.CreatePedidoPersonalizado;

public class CreatePedidoPersonalizadoHandler(
    IPedidoRepository pedidoRepo,
    INotificacaoRepository notifRepo,
    IUnitOfWork uow) : IRequestHandler<CreatePedidoPersonalizadoCommand, PedidoDto>
{
    public async Task<PedidoDto> Handle(CreatePedidoPersonalizadoCommand cmd, CancellationToken ct)
    {
        var agora = DateTime.UtcNow;
        var pedido = new PedidoPersonalizado
        {
            Id = Guid.NewGuid(),
            Tipo = TipoPedido.Personalizado,
            ClienteId = cmd.ClienteId,
            CompradorId = string.Empty,
            Endereco = cmd.Endereco,
            Status = StatusPedido.Pendente,
            DataCriacao = agora,
            PrazoAceite = agora.AddHours(24),
            Especie = cmd.Especie,
            QuantidadeTotal = cmd.QuantidadeTotal,
            DataLimite = cmd.DataLimite,
            Observacoes = cmd.Observacoes,
            StatusPagamento = "pendente"
        };
        await pedidoRepo.AddAsync(pedido, ct);

        await notifRepo.AddAsync(new Notificacao
        {
            Id = Guid.NewGuid(),
            UsuarioId = "comp-1",
            Tipo = TipoNotificacao.Pedido,
            Titulo = "Novo pedido personalizado",
            Mensagem = $"Solicitação: {cmd.QuantidadeTotal} de {cmd.Especie} até {cmd.DataLimite:dd/MM}",
            Lida = false,
            Timestamp = agora
        }, ct);

        await uow.SaveChangesAsync(ct);

        return new PedidoDto(pedido.Id, pedido.Tipo.ToString(), pedido.ClienteId,
            pedido.CompradorId, pedido.Endereco, pedido.Status.ToString(),
            pedido.DataCriacao, pedido.DataEntrega, null, pedido.StatusPagamento,
            [], pedido.Especie, pedido.QuantidadeTotal, pedido.DataLimite,
            pedido.Observacoes, pedido.PrazoAceite);
    }
}
