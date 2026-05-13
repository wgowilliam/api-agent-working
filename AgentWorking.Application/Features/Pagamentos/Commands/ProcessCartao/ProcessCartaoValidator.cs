using FluentValidation;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessCartao;
public class ProcessCartaoValidator : AbstractValidator<ProcessCartaoCommand>
{
    public ProcessCartaoValidator()
    {
        RuleFor(x => x.NumeroCartao).NotEmpty().Length(16)
            .Matches(@"^\d{16}$").WithMessage("Número do cartão deve ter 16 dígitos");
        RuleFor(x => x.NomeTitular).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Validade).NotEmpty().Matches(@"^\d{2}/\d{2}$")
            .WithMessage("Validade deve ser MM/AA");
        RuleFor(x => x.Cvv).NotEmpty().Length(3, 4).Matches(@"^\d{3,4}$");
        RuleFor(x => x.Valor).GreaterThan(0);
    }
}
