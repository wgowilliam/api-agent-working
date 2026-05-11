using AgentWorking.Application.Features.Produtos.Commands.CreateProduto;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Moq;

namespace AgentWorking.Tests.Application.Features.Produtos;

public class CreateProdutoHandlerTests
{
    private readonly Mock<IProdutoRepository> _repoMock = new();
    private readonly Mock<ICentroRepository> _centroMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    [Fact]
    public async Task Handle_ValidCommand_CreatesProdutoWithStatusAtivo()
    {
        var centroId = Guid.NewGuid();
        _centroMock.Setup(r => r.ExistsAsync(centroId, default)).ReturnsAsync(true);

        Produto? captured = null;
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Produto>(), default))
            .Callback<Produto, CancellationToken>((p, _) => captured = p)
            .Returns(Task.CompletedTask);

        var handler = new CreateProdutoHandler(_repoMock.Object, _centroMock.Object, _uowMock.Object);

        var result = await handler.Handle(new CreateProdutoCommand(
            Nome: "Tomate Carmem",
            Categoria: Categoria.Verduras,
            Quantidade: 150,
            Unidade: UnidadeMedida.Kg,
            Preco: 4.5m,
            Safra: "2026-05",
            Cidade: "Campinas",
            ProdutorId: "prod-1",
            CentroDistribuicaoId: centroId,
            Foto: null), default);

        Assert.NotNull(captured);
        Assert.Equal("Tomate Carmem", captured.Nome);
        Assert.Equal(StatusOferta.Ativo, captured.Status);
        Assert.Equal(centroId, captured.CentroDistribuicaoId);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCentro_ThrowsKeyNotFoundException()
    {
        _centroMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), default)).ReturnsAsync(false);
        var handler = new CreateProdutoHandler(_repoMock.Object, _centroMock.Object, _uowMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new CreateProdutoCommand(
                "X", Categoria.Frutas, 1, UnidadeMedida.Kg, 1m,
                "2026-05", "SP", "prod-1", Guid.NewGuid(), null), default));
    }
}
