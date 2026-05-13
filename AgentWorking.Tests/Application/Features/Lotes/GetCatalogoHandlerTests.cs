using AgentWorking.Application.Features.Lotes.Queries.GetCatalogo;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Moq;

namespace AgentWorking.Tests.Application.Features.Lotes;

public class GetCatalogoHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsMappedLoteDtos()
    {
        var mock = new Mock<ILoteRepository>();
        var lotes = new List<LoteEstoque>
        {
            new() {
                Id = Guid.NewGuid(), CompraId = Guid.NewGuid(),
                ProdutoId = Guid.NewGuid(), Nome = "Tomate",
                Quantidade = 80, Validade = DateTime.UtcNow.AddDays(10),
                PrecoVenda = 5.8m, CompradorId = "comp-1"
            }
        };
        mock.Setup(r => r.GetCatalogoAsync(null, default)).ReturnsAsync(lotes);

        var handler = new GetCatalogoHandler(mock.Object);
        var result = await handler.Handle(new GetCatalogoQuery(null), default);

        Assert.Single(result);
        Assert.Equal("Tomate", result[0].Nome);
    }
}
