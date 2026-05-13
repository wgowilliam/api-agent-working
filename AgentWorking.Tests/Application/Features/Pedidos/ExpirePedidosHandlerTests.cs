using AgentWorking.Application.Features.Pedidos.Commands.ExpirePedidos;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Moq;

namespace AgentWorking.Tests.Application.Features.Pedidos;

public class ExpirePedidosHandlerTests
{
    [Fact]
    public async Task Handle_ExpiredPedidos_SetsStatusExpiradoAndNotifiesCliente()
    {
        var pedidoMock = new Mock<IPedidoRepository>();
        var notifMock = new Mock<INotificacaoRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        var expired = new PedidoPersonalizado
        {
            Id = Guid.NewGuid(), ClienteId = "cli-1", CompradorId = "comp-1",
            Endereco = "X", Status = StatusPedido.Pendente,
            DataCriacao = DateTime.UtcNow.AddHours(-25),
            PrazoAceite = DateTime.UtcNow.AddHours(-1),
            Especie = "Abacate", QuantidadeTotal = 500,
            DataLimite = DateTime.UtcNow.AddDays(5),
            Observacoes = string.Empty, Tipo = TipoPedido.Personalizado
        };
        pedidoMock.Setup(r => r.GetExpiredPersonalizadosAsync(default))
            .ReturnsAsync([expired]);

        var handler = new ExpirePedidosHandler(pedidoMock.Object, notifMock.Object, uowMock.Object);
        await handler.Handle(new ExpirePedidosCommand(), default);

        Assert.Equal(StatusPedido.Expirado, expired.Status);
        notifMock.Verify(r => r.AddAsync(
            It.Is<Notificacao>(n => n.UsuarioId == "cli-1" && n.Tipo == TipoNotificacao.Alerta),
            default), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_NoExpiredPedidos_DoesNotSaveChanges()
    {
        var pedidoMock = new Mock<IPedidoRepository>();
        var notifMock = new Mock<INotificacaoRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        pedidoMock.Setup(r => r.GetExpiredPersonalizadosAsync(default))
            .ReturnsAsync([]);

        var handler = new ExpirePedidosHandler(pedidoMock.Object, notifMock.Object, uowMock.Object);
        await handler.Handle(new ExpirePedidosCommand(), default);

        uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }
}
