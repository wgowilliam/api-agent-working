using FluentValidation;
namespace AgentWorking.Application.Features.Compras.Commands.CreateCompra;
public class CreateCompraValidator : AbstractValidator<CreateCompraCommand>
{
    public CreateCompraValidator()
    {
        RuleFor(x => x.ProdutorId).NotEmpty();
        RuleFor(x => x.CompradorId).NotEmpty();
        RuleFor(x => x.ProdutoId).NotEqual(Guid.Empty);
        RuleFor(x => x.Quantidade).GreaterThan(0);
        RuleFor(x => x.PrecoUnitario).GreaterThan(0);
        RuleFor(x => x.PrecoVenda).GreaterThan(0);
        RuleFor(x => x.Validade).GreaterThan(DateTime.UtcNow);
    }
}
