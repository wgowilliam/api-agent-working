using AgentWorking.Application.Features.Pedidos.Commands.CreatePedido;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Moq;

namespace AgentWorking.Tests.Application.Features.Pedidos;

public class CreatePedidoHandlerTests
{
    private readonly Mock<IPedidoRepository> _pedidoMock = new();
    private readonly Mock<ILoteRepository> _loteMock = new();
    private readonly Mock<IVendaRepository> _vendaMock = new();
    private readonly Mock<IEntregaRepository> _entregaMock = new();
    private readonly Mock<INotificacaoRepository> _notifMock = new();
    private readonly Mock<ICompraRepository> _compraMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private CreatePedidoHandler BuildHandler() => new(
        _pedidoMock.Object, _loteMock.Object, _vendaMock.Object,
        _entregaMock.Object, _notifMock.Object, _compraMock.Object, _uowMock.Object);

    [Fact]
    public async Task Handle_ValidPedido_DecreasesLoteAndCreatesVenda()
    {
        var loteId = Guid.NewGuid();
        var compraId = Guid.NewGuid();
        var lote = new LoteEstoque
        {
            Id = loteId, CompraId = compraId, ProdutoId = Guid.NewGuid(),
            Nome = "Tomate", Quantidade = 80, Validade = DateTime.UtcNow.AddDays(10),
            PrecoVenda = 5.8m, CompradorId = "comp-1"
        };
        var compra = new Compra
        {
            Id = compraId, ProdutorId = "prod-1", CompradorId = "comp-1",
            ProdutoId = lote.ProdutoId, Quantidade = 80, PrecoUnitario = 4.5m,
            DataCompra = DateTime.UtcNow
        };
        _loteMock.Setup(r => r.GetByIdAsync(loteId, default)).ReturnsAsync(lote);
        _compraMock.Setup(r => r.GetByIdAsync(compraId, default)).ReturnsAsync(compra);

        Venda? capturedVenda = null;
        _vendaMock.Setup(r => r.AddAsync(It.IsAny<Venda>(), default))
            .Callback<Venda, CancellationToken>((v, _) => capturedVenda = v)
            .Returns(Task.CompletedTask);

        var cmd = new CreatePedidoCommand(
            ClienteId: "cli-1", CompradorId: "comp-1",
            Endereco: "Rua X, 123",
            MetodoPagamento: MetodoPagamento.Pix,
            Itens: [new CreatePedidoItemDto(loteId, "Tomate", 30, 5.8m)]);

        var result = await BuildHandler().Handle(cmd, default);

        Assert.Equal(50, lote.Quantidade); // 80 - 30 = 50
        Assert.NotNull(capturedVenda);
        Assert.Equal("prod-1", capturedVenda.ProdutorId);
        Assert.Equal(30 * 5.8m, capturedVenda.ValorTotal);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_InsufficientLoteQty_ThrowsInvalidOperationException()
    {
        var loteId = Guid.NewGuid();
        _loteMock.Setup(r => r.GetByIdAsync(loteId, default)).ReturnsAsync(
            new LoteEstoque
            {
                Id = loteId, CompraId = Guid.NewGuid(), ProdutoId = Guid.NewGuid(),
                Nome = "X", Quantidade = 5, Validade = DateTime.UtcNow.AddDays(5),
                PrecoVenda = 1m, CompradorId = "comp-1"
            });

        var cmd = new CreatePedidoCommand("cli-1", "comp-1", "Rua X",
            MetodoPagamento.Pix, [new CreatePedidoItemDto(loteId, "X", 50, 1m)]);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => BuildHandler().Handle(cmd, default));
    }
}
