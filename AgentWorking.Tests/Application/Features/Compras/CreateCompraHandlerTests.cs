using AgentWorking.Application.Features.Compras.Commands.CreateCompra;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Moq;

namespace AgentWorking.Tests.Application.Features.Compras;

public class CreateCompraHandlerTests
{
    private readonly Mock<IProdutoRepository> _produtoMock = new();
    private readonly Mock<ICompraRepository> _compraMock = new();
    private readonly Mock<ILoteRepository> _loteMock = new();
    private readonly Mock<INotificacaoRepository> _notifMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    [Fact]
    public async Task Handle_ValidCompra_CreatesCompraAndLote()
    {
        var produtoId = Guid.NewGuid();
        var produto = new Produto
        {
            Id = produtoId, Nome = "Tomate", Quantidade = 150,
            Preco = 4.5m, ProdutorId = "prod-1",
            Categoria = Categoria.Verduras, Unidade = UnidadeMedida.Kg,
            Safra = "2026-05", Cidade = "SP", Status = StatusOferta.Ativo,
            CentroDistribuicaoId = Guid.NewGuid()
        };
        _produtoMock.Setup(r => r.GetByIdAsync(produtoId, default)).ReturnsAsync(produto);

        LoteEstoque? capturedLote = null;
        _loteMock.Setup(r => r.AddAsync(It.IsAny<LoteEstoque>(), default))
            .Callback<LoteEstoque, CancellationToken>((l, _) => capturedLote = l)
            .Returns(Task.CompletedTask);

        var handler = new CreateCompraHandler(
            _produtoMock.Object, _compraMock.Object,
            _loteMock.Object, _notifMock.Object, _uowMock.Object);

        var validade = DateTime.UtcNow.AddDays(30);
        var result = await handler.Handle(new CreateCompraCommand(
            CompradorId: "comp-1", ProdutoId: produtoId,
            Quantidade: 50, PrecoUnitario: 4.5m,
            PrecoVenda: 6.0m, Validade: validade), default);

        Assert.NotNull(capturedLote);
        Assert.Equal(50, capturedLote.Quantidade);
        Assert.Equal(validade, capturedLote.Validade);
        Assert.Equal(6.0m, capturedLote.PrecoVenda);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_InsufficientStock_ThrowsInvalidOperationException()
    {
        var produtoId = Guid.NewGuid();
        _produtoMock.Setup(r => r.GetByIdAsync(produtoId, default)).ReturnsAsync(
            new Produto { Id = produtoId, Quantidade = 10, ProdutorId = "prod-1",
                Nome = "X", Safra = "2026", Cidade = "SP",
                Categoria = Categoria.Frutas, Unidade = UnidadeMedida.Kg });

        var handler = new CreateCompraHandler(
            _produtoMock.Object, _compraMock.Object,
            _loteMock.Object, _notifMock.Object, _uowMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new CreateCompraCommand(
                "comp-1", produtoId, 50, 4.5m, 6m,
                DateTime.UtcNow.AddDays(30)), default));
    }
}
