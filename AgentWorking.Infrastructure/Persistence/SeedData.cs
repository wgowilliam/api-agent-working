using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Centros.AnyAsync()) return;

        var centroSp = new CentroDistribuicao
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            Nome = "CEAGESP",
            Endereco = "Av. Dr. Gastão Vidigal, 1946",
            Cidade = "São Paulo"
        };
        var centroCamp = new CentroDistribuicao
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
            Nome = "CEASA Campinas",
            Endereco = "Rod. Dom Pedro I, km 134",
            Cidade = "Campinas"
        };
        context.Centros.AddRange(centroSp, centroCamp);

        var produtos = new List<Produto>
        {
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Nome = "Tomate Carmem", Categoria = Categoria.Verduras, Quantidade = 150, Unidade = UnidadeMedida.Kg, Preco = 4.5m, Safra = "2026-05", Cidade = "Campinas", ProdutorId = "prod-1", Status = StatusOferta.Ativo, CentroDistribuicaoId = centroSp.Id },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Nome = "Alface Crespa", Categoria = Categoria.Verduras, Quantidade = 80, Unidade = UnidadeMedida.Un, Preco = 2.8m, Safra = "2026-05", Cidade = "Campinas", ProdutorId = "prod-1", Status = StatusOferta.Ativo, CentroDistribuicaoId = centroSp.Id },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Nome = "Cenoura", Categoria = Categoria.Legumes, Quantidade = 200, Unidade = UnidadeMedida.Kg, Preco = 3.2m, Safra = "2026-04", Cidade = "Itu", ProdutorId = "prod-1", Status = StatusOferta.Pausado, CentroDistribuicaoId = centroCamp.Id },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), Nome = "Banana Prata", Categoria = Categoria.Frutas, Quantidade = 300, Unidade = UnidadeMedida.Kg, Preco = 5.5m, Safra = "2026-05", Cidade = "Registro", ProdutorId = "prod-1", Status = StatusOferta.Ativo, CentroDistribuicaoId = centroSp.Id },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000005"), Nome = "Batata Inglesa", Categoria = Categoria.Legumes, Quantidade = 500, Unidade = UnidadeMedida.Kg, Preco = 3.8m, Safra = "2026-03", Cidade = "Itapetininga", ProdutorId = "prod-1", Status = StatusOferta.Ativo, CentroDistribuicaoId = centroCamp.Id },
        };
        context.Produtos.AddRange(produtos);

        await context.SaveChangesAsync();
    }
}
