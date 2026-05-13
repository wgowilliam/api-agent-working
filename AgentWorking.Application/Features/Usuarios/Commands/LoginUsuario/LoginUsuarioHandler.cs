using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;

namespace AgentWorking.Application.Features.Usuarios.Commands.LoginUsuario;

public class LoginUsuarioHandler(
    IUsuarioRepository repo,
    IPasswordHasher hasher,
    ITokenService tokenService) : IRequestHandler<LoginUsuarioCommand, TokenDto>
{
    public async Task<TokenDto> Handle(LoginUsuarioCommand cmd, CancellationToken ct)
    {
        var user = await repo.GetByEmailAsync(cmd.Email, ct)
            ?? throw new UnauthorizedAccessException("Credenciais inválidas.");

        if (!hasher.Verify(cmd.Senha, user.SenhaHash))
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        var token = tokenService.GenerateToken(user);
        var expiry = tokenService.GetExpiry();
        var userDto = new UserDto(user.Id, user.Nome, user.Email, user.Tipo.ToString(), user.DataCadastro);

        return new TokenDto(token, expiry, userDto);
    }
}
