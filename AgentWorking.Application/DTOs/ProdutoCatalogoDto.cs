namespace AgentWorking.Application.DTOs;

public record ProdutoCatalogoDto(
    Guid Id,
    string Nome,
    string Categoria,
    decimal Quantidade,
    string Unidade,
    decimal Preco,
    string Safra,
    string Cidade,
    string? Foto,
    string ProdutorId,
    Guid CentroDistribuicaoId,
    string CentroNome,
    string CentroCidade);
