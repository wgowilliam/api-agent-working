namespace AgentWorking.Application.DTOs;
public record ProdutoDto(
    Guid Id, string Nome, string Categoria, decimal Quantidade,
    string Unidade, decimal Preco, string Safra, string Cidade,
    string? Foto, string ProdutorId, string Status,
    Guid CentroDistribuicaoId);
