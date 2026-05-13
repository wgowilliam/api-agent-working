using FluentValidation;
namespace AgentWorking.Application.Features.Pedidos.Commands.CreatePedidoPersonalizado;
public class CreatePedidoPersonalizadoValidator : AbstractValidator<CreatePedidoPersonalizadoCommand>
{
    public CreatePedidoPersonalizadoValidator()
    {
        RuleFor(x => x.ClienteId).NotEmpty();
        RuleFor(x => x.Endereco).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Especie).NotEmpty().MaximumLength(200);
        RuleFor(x => x.QuantidadeTotal).GreaterThan(0);
        RuleFor(x => x.DataLimite).GreaterThan(DateTime.UtcNow);
    }
}
