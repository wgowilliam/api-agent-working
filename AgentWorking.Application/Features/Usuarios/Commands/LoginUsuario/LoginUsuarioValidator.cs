using FluentValidation;

namespace AgentWorking.Application.Features.Usuarios.Commands.LoginUsuario;

public class LoginUsuarioValidator : AbstractValidator<LoginUsuarioCommand>
{
    public LoginUsuarioValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Senha).NotEmpty();
    }
}
