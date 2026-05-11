using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Commands.PatchProdutoStatus;
public record PatchProdutoStatusCommand(Guid Id) : IRequest<ProdutoDto>;
