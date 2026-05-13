using FluentValidation;
namespace AgentWorking.Application.Features.Pedidos.Commands.CreatePedido;
public class CreatePedidoValidator : AbstractValidator<CreatePedidoCommand>
{
    public CreatePedidoValidator()
    {
        RuleFor(x => x.ClienteId).NotEmpty();
        RuleFor(x => x.CompradorId).NotEmpty();
        RuleFor(x => x.Endereco).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Itens).NotEmpty().WithMessage("Pedido deve ter ao menos 1 item");
        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.LoteId).NotEqual(Guid.Empty);
            item.RuleFor(i => i.Quantidade).GreaterThan(0);
            item.RuleFor(i => i.Preco).GreaterThan(0);
        });
    }
}
