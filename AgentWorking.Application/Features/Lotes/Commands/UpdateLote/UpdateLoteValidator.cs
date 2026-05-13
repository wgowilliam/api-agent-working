using FluentValidation;
namespace AgentWorking.Application.Features.Lotes.Commands.UpdateLote;
public class UpdateLoteValidator : AbstractValidator<UpdateLoteCommand>
{
    public UpdateLoteValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty);
        RuleFor(x => x.Quantidade).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PrecoVenda).GreaterThan(0);
        RuleFor(x => x.Validade).GreaterThan(DateTime.UtcNow);
    }
}
