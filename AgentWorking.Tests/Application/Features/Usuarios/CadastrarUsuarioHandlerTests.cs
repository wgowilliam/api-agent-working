using AgentWorking.Application.Features.Usuarios.Commands.CadastrarUsuario;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Moq;

namespace AgentWorking.Tests.Application.Features.Usuarios;

public class CadastrarUsuarioHandlerTests
{
    private readonly Mock<IUsuarioRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();

    public CadastrarUsuarioHandlerTests()
    {
        _hasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed");
    }

    private CadastrarUsuarioHandler BuildHandler() =>
        new(_repoMock.Object, _uowMock.Object, _hasherMock.Object);

    [Fact]
    public async Task Handle_NewEmail_CreatesAndReturnsUser()
    {
        _repoMock.Setup(r => r.GetByEmailAsync("novo@test.com", default))
            .ReturnsAsync((User?)null);

        var cmd = new CadastrarUsuarioCommand("João", "novo@test.com", "senha123", TipoUsuario.Produtor);
        var result = await BuildHandler().Handle(cmd, default);

        Assert.Equal("João", result.Nome);
        Assert.Equal("novo@test.com", result.Email);
        Assert.Equal("Produtor", result.Tipo);
        Assert.NotEqual(Guid.Empty, result.Id);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var existing = new User
        {
            Id = Guid.NewGuid(), Nome = "Existing", Email = "dup@test.com",
            Tipo = TipoUsuario.Comprador, DataCadastro = DateTime.UtcNow
        };
        _repoMock.Setup(r => r.GetByEmailAsync("dup@test.com", default))
            .ReturnsAsync(existing);

        var cmd = new CadastrarUsuarioCommand("Outro", "dup@test.com", "senha123", TipoUsuario.Cliente);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => BuildHandler().Handle(cmd, default));

        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }
}
