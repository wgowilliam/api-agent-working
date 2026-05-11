using FluentValidation;
namespace AgentWorking.Application.Features.Produtos.Commands.CreateProduto;

public class CreateProdutoValidator : AbstractValidator<CreateProdutoCommand>
{
    public CreateProdutoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Quantidade).GreaterThan(0);
        RuleFor(x => x.Preco).GreaterThan(0);
        RuleFor(x => x.Safra).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Cidade).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ProdutorId).NotEmpty();
        RuleFor(x => x.CentroDistribuicaoId).NotEqual(Guid.Empty);
    }
}
