using FluentValidation;
namespace AgentWorking.Application.Features.Produtos.Commands.UpdateProduto;
public class UpdateProdutoValidator : AbstractValidator<UpdateProdutoCommand>
{
    public UpdateProdutoValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty);
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Quantidade).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Preco).GreaterThan(0);
    }
}
