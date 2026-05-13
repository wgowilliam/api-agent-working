using FluentValidation;

namespace AgentWorking.Application.Features.Usuarios.Commands.CadastrarUsuario;

public class CadastrarUsuarioValidator : AbstractValidator<CadastrarUsuarioCommand>
{
    public CadastrarUsuarioValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
    }
}
