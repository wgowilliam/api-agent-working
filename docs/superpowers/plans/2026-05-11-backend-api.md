# Portal Agro B2B/B2C — Backend Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the complete .NET 8 REST API for the Portal Agro B2B/B2C, covering Centros de Distribuição, Produtos, Compras, Lotes, Pedidos (padrão + personalizado), Vendas, Entregas, Notificações, Pagamentos, and a background job for 24h order expiry.

**Architecture:** Clean Architecture with MediatR CQRS. Controllers are thin (MediatR.Send only). Business logic lives in Application handlers. EF Core 8 + Npgsql on PostgreSQL. Repository + UnitOfWork pattern. ValidationBehavior pipeline with FluentValidation.

**Tech Stack:** .NET 8, ASP.NET Core, EF Core 8, Npgsql 8, MediatR 12, FluentValidation 11, xUnit 2.5, Moq 4, Swashbuckle 6, Docker Compose (PostgreSQL already configured)

---

## File Map

```
AgentWorking.Domain/
  Entities/
    CentroDistribuicao.cs     ← CD entity
    Produto.cs                ← + Cidade, CentroDistribuicaoId
    LoteEstoque.cs            ← Validade required
    Compra.cs                 ← Comprador buys from Produtor → creates Lote
    Pedido.cs                 ← base order (TPH root)
    PedidoPersonalizado.cs    ← extends Pedido (TPH leaf)
    ItemPedido.cs             ← separate table, refs LoteId
    Venda.cs                  ← sale record for Produtor history
    EntregaStatus.cs          ← 3-step delivery tracking
    Notificacao.cs            ← in-app notification
  Enums/
    Categoria.cs
    UnidadeMedida.cs
    StatusOferta.cs
    StatusPedido.cs
    StatusEntrega.cs
    MetodoPagamento.cs
    TipoPedido.cs
    TipoNotificacao.cs
  Interfaces/
    IRepository.cs            ← generic base interface

AgentWorking.Application/
  Interfaces/
    IUnitOfWork.cs
    ICentroRepository.cs
    IProdutoRepository.cs
    ILoteRepository.cs
    ICompraRepository.cs
    IPedidoRepository.cs
    IVendaRepository.cs
    IEntregaRepository.cs
    INotificacaoRepository.cs
  DTOs/
    CentroDto.cs
    ProdutoDto.cs
    LoteDto.cs
    CompraDto.cs
    PedidoDto.cs
    VendaDto.cs
    EntregaDto.cs
    NotificacaoDto.cs
    PagamentoDto.cs
  Behaviors/
    ValidationBehavior.cs
  Features/
    Centros/Queries/GetCentros/
      GetCentrosQuery.cs + GetCentrosHandler.cs
    Produtos/
      Queries/GetProdutos/GetProdutosQuery.cs + GetProdutosHandler.cs
      Commands/CreateProduto/CreateProdutoCommand.cs + Handler + Validator
      Commands/UpdateProduto/UpdateProdutoCommand.cs + Handler + Validator
      Commands/PatchProdutoStatus/PatchProdutoStatusCommand.cs + Handler
    Compras/
      Commands/CreateCompra/CreateCompraCommand.cs + Handler + Validator
      Queries/GetCompras/GetComprasQuery.cs + Handler
    Lotes/
      Queries/GetLotes/GetLotesQuery.cs + Handler
      Queries/GetCatalogo/GetCatalogoQuery.cs + Handler
      Commands/UpdateLote/UpdateLoteCommand.cs + Handler + Validator
    Pedidos/
      Queries/GetPedidos/GetPedidosQuery.cs + Handler
      Commands/CreatePedido/CreatePedidoCommand.cs + Handler + Validator
      Commands/CreatePedidoPersonalizado/CreatePedidoPersonalizadoCommand.cs + Handler + Validator
      Commands/PatchPedidoStatus/PatchPedidoStatusCommand.cs + Handler
      Commands/ExpirePedidos/ExpirePedidosCommand.cs + Handler
    Vendas/Queries/GetVendas/GetVendasQuery.cs + Handler
    Entregas/
      Queries/GetEntrega/GetEntregaQuery.cs + Handler
      Commands/PatchEntregaStatus/PatchEntregaStatusCommand.cs + Handler
    Notificacoes/
      Queries/GetNotificacoes/GetNotificacoesQuery.cs + Handler
      Commands/PatchNotificacaoLida/PatchNotificacaoLidaCommand.cs + Handler
      Commands/PatchTodasLidas/PatchTodasLidasCommand.cs + Handler
      Commands/CreateNotificacao/CreateNotificacaoCommand.cs + Handler  ← internal only
    Pagamentos/
      Commands/ProcessPix/ProcessPixCommand.cs + Handler
      Commands/ProcessCartao/ProcessCartaoCommand.cs + Handler + Validator
      Commands/ProcessBoleto/ProcessBoletoCommand.cs + Handler
  DependencyInjection.cs      ← extension method registers all Application services

AgentWorking.Infrastructure/
  Persistence/
    AppDbContext.cs
    Configurations/
      CentroDistribuicaoConfiguration.cs
      ProdutoConfiguration.cs
      LoteEstoqueConfiguration.cs
      CompraConfiguration.cs
      PedidoConfiguration.cs    ← TPH discriminator here
      ItemPedidoConfiguration.cs
      VendaConfiguration.cs
      EntregaStatusConfiguration.cs
      NotificacaoConfiguration.cs
    Repositories/
      Repository.cs             ← generic base
      CentroRepository.cs
      ProdutoRepository.cs
      LoteRepository.cs
      CompraRepository.cs
      PedidoRepository.cs
      VendaRepository.cs
      EntregaRepository.cs
      NotificacaoRepository.cs
    UnitOfWork.cs
    SeedData.cs                 ← matches src/mocks/data.ts
  BackgroundJobs/
    PedidoExpirationJob.cs      ← IHostedService, runs every 5 min
  DependencyInjection.cs        ← extension method registers Infrastructure services

AgentWorking.API/
  Controllers/
    CentrosController.cs
    ProdutosController.cs
    ComprasController.cs
    LotesController.cs
    PedidosController.cs
    VendasController.cs
    EntregasController.cs
    PagamentosController.cs
    NotificacoesController.cs
  Middleware/
    ExceptionHandlerMiddleware.cs
  Program.cs                    ← updated: DI, CORS, Swagger, Problem Details
  appsettings.Development.json  ← connection string

AgentWorking.Tests/
  AgentWorking.Tests.csproj     ← add Moq + Infrastructure ref
  Application/Features/
    Centros/GetCentrosHandlerTests.cs
    Produtos/CreateProdutoHandlerTests.cs
    Compras/CreateCompraHandlerTests.cs
    Lotes/GetCatalogoHandlerTests.cs
    Pedidos/CreatePedidoHandlerTests.cs
    Pedidos/ExpirePedidosHandlerTests.cs
```

---

## Task 1: Install packages

**Files:**
- Modify: `AgentWorking.Domain/AgentWorking.Domain.csproj`
- Modify: `AgentWorking.Application/AgentWorking.Application.csproj`
- Modify: `AgentWorking.Infrastructure/AgentWorking.Infrastructure.csproj`
- Modify: `AgentWorking.Tests/AgentWorking.Tests.csproj`

- [ ] **Step 1: Add packages to Domain (none needed — pure C#)**

Domain has no external dependencies. Skip.

- [ ] **Step 2: Add packages to Application**

```bash
cd /root/api-agent-working/AgentWorking.Application
dotnet add package MediatR --version 12.4.1
dotnet add package FluentValidation --version 11.11.0
dotnet add package FluentValidation.DependencyInjectionExtensions --version 11.11.0
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions --version 8.0.2
```

- [ ] **Step 3: Add packages to Infrastructure**

```bash
cd /root/api-agent-working/AgentWorking.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.15
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.15
dotnet add package Microsoft.Extensions.Hosting.Abstractions --version 8.0.1
```

- [ ] **Step 4: Add packages to Tests**

```bash
cd /root/api-agent-working/AgentWorking.Tests
dotnet add package Moq --version 4.20.72
```

Add Infrastructure project reference to Tests:

```xml
<!-- AgentWorking.Tests/AgentWorking.Tests.csproj — add inside <ItemGroup> with ProjectReferences -->
<ProjectReference Include="..\AgentWorking.Infrastructure\AgentWorking.Infrastructure.csproj" />
```

- [ ] **Step 5: Add EF tools to Infrastructure (for migrations)**

```bash
cd /root/api-agent-working
dotnet tool install --global dotnet-ef --version 8.0.15 2>/dev/null || dotnet tool update --global dotnet-ef
```

- [ ] **Step 6: Verify solution builds**

```bash
cd /root/api-agent-working
dotnet build
```

Expected: `Build succeeded. 0 Warning(s). 0 Error(s)` (or only warnings about empty Class1.cs files — acceptable)

- [ ] **Step 7: Commit**

```bash
git -C /root/api-agent-working add -A
git -C /root/api-agent-working commit -m "chore: add NuGet packages for EF Core, MediatR, FluentValidation, Moq"
```

---

## Task 2: Domain enums

**Files:**
- Create: `AgentWorking.Domain/Enums/Categoria.cs`
- Create: `AgentWorking.Domain/Enums/UnidadeMedida.cs`
- Create: `AgentWorking.Domain/Enums/StatusOferta.cs`
- Create: `AgentWorking.Domain/Enums/StatusPedido.cs`
- Create: `AgentWorking.Domain/Enums/StatusEntrega.cs`
- Create: `AgentWorking.Domain/Enums/MetodoPagamento.cs`
- Create: `AgentWorking.Domain/Enums/TipoPedido.cs`
- Create: `AgentWorking.Domain/Enums/TipoNotificacao.cs`

- [ ] **Step 1: Create all enum files**

```csharp
// AgentWorking.Domain/Enums/Categoria.cs
namespace AgentWorking.Domain.Enums;
public enum Categoria { Frutas, Verduras, Legumes, Graos, Outros }
```

```csharp
// AgentWorking.Domain/Enums/UnidadeMedida.cs
namespace AgentWorking.Domain.Enums;
public enum UnidadeMedida { Kg, Cx, Un }
```

```csharp
// AgentWorking.Domain/Enums/StatusOferta.cs
namespace AgentWorking.Domain.Enums;
public enum StatusOferta { Ativo, Pausado, Esgotado }
```

```csharp
// AgentWorking.Domain/Enums/StatusPedido.cs
namespace AgentWorking.Domain.Enums;
public enum StatusPedido { Pendente, Confirmado, Recusado, EmEntrega, Entregue, Expirado }
```

```csharp
// AgentWorking.Domain/Enums/StatusEntrega.cs
namespace AgentWorking.Domain.Enums;
public enum StatusEntrega { Saiu, EmTransporte, Entregue }
```

```csharp
// AgentWorking.Domain/Enums/MetodoPagamento.cs
namespace AgentWorking.Domain.Enums;
public enum MetodoPagamento { Pix, Cartao, Boleto }
```

```csharp
// AgentWorking.Domain/Enums/TipoPedido.cs
namespace AgentWorking.Domain.Enums;
public enum TipoPedido { Padrao, Personalizado }
```

```csharp
// AgentWorking.Domain/Enums/TipoNotificacao.cs
namespace AgentWorking.Domain.Enums;
public enum TipoNotificacao { Pedido, Pagamento, Entrega, Alerta }
```

- [ ] **Step 2: Verify build**

```bash
cd /root/api-agent-working && dotnet build AgentWorking.Domain
```

Expected: `Build succeeded.`

- [ ] **Step 3: Commit**

```bash
git -C /root/api-agent-working add AgentWorking.Domain/Enums/
git -C /root/api-agent-working commit -m "feat(domain): add domain enums"
```

---

## Task 3: Domain entities

**Files:**
- Create: `AgentWorking.Domain/Entities/CentroDistribuicao.cs`
- Create: `AgentWorking.Domain/Entities/Produto.cs`
- Create: `AgentWorking.Domain/Entities/LoteEstoque.cs`
- Create: `AgentWorking.Domain/Entities/Compra.cs`
- Create: `AgentWorking.Domain/Entities/Pedido.cs`
- Create: `AgentWorking.Domain/Entities/PedidoPersonalizado.cs`
- Create: `AgentWorking.Domain/Entities/ItemPedido.cs`
- Create: `AgentWorking.Domain/Entities/Venda.cs`
- Create: `AgentWorking.Domain/Entities/EntregaStatus.cs`
- Create: `AgentWorking.Domain/Entities/Notificacao.cs`
- Create: `AgentWorking.Domain/Interfaces/IRepository.cs`

- [ ] **Step 1: Create IRepository interface**

```csharp
// AgentWorking.Domain/Interfaces/IRepository.cs
namespace AgentWorking.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
}
```

- [ ] **Step 2: Create CentroDistribuicao**

```csharp
// AgentWorking.Domain/Entities/CentroDistribuicao.cs
namespace AgentWorking.Domain.Entities;

public class CentroDistribuicao
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public ICollection<Produto> Produtos { get; set; } = [];
}
```

- [ ] **Step 3: Create Produto**

```csharp
// AgentWorking.Domain/Entities/Produto.cs
using AgentWorking.Domain.Enums;

namespace AgentWorking.Domain.Entities;

public class Produto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Categoria Categoria { get; set; }
    public decimal Quantidade { get; set; }
    public UnidadeMedida Unidade { get; set; }
    public decimal Preco { get; set; }
    public string Safra { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string? Foto { get; set; }
    public string ProdutorId { get; set; } = string.Empty;
    public StatusOferta Status { get; set; } = StatusOferta.Ativo;
    public Guid CentroDistribuicaoId { get; set; }
    public CentroDistribuicao CentroDistribuicao { get; set; } = null!;
    public ICollection<Compra> Compras { get; set; } = [];
}
```

- [ ] **Step 4: Create LoteEstoque**

```csharp
// AgentWorking.Domain/Entities/LoteEstoque.cs
namespace AgentWorking.Domain.Entities;

public class LoteEstoque
{
    public Guid Id { get; set; }
    public Guid CompraId { get; set; }
    public Guid ProdutoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public DateTime Validade { get; set; }
    public decimal PrecoVenda { get; set; }
    public string CompradorId { get; set; } = string.Empty;
    public Compra Compra { get; set; } = null!;
    public Produto Produto { get; set; } = null!;
    public ICollection<ItemPedido> ItensPedido { get; set; } = [];
}
```

- [ ] **Step 5: Create Compra**

```csharp
// AgentWorking.Domain/Entities/Compra.cs
namespace AgentWorking.Domain.Entities;

public class Compra
{
    public Guid Id { get; set; }
    public string ProdutorId { get; set; } = string.Empty;
    public string CompradorId { get; set; } = string.Empty;
    public Guid ProdutoId { get; set; }
    public decimal Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public DateTime DataCompra { get; set; }
    public Produto Produto { get; set; } = null!;
    public LoteEstoque? Lote { get; set; }
}
```

- [ ] **Step 6: Create Pedido (TPH base)**

```csharp
// AgentWorking.Domain/Entities/Pedido.cs
using AgentWorking.Domain.Enums;

namespace AgentWorking.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; }
    public TipoPedido Tipo { get; set; }
    public string ClienteId { get; set; } = string.Empty;
    public string CompradorId { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public StatusPedido Status { get; set; } = StatusPedido.Pendente;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataEntrega { get; set; }
    public MetodoPagamento? MetodoPagamento { get; set; }
    public string StatusPagamento { get; set; } = "pendente";
    public ICollection<ItemPedido> Itens { get; set; } = [];
    public EntregaStatus? Entrega { get; set; }
    public ICollection<Venda> Vendas { get; set; } = [];
}
```

- [ ] **Step 7: Create PedidoPersonalizado (TPH leaf)**

```csharp
// AgentWorking.Domain/Entities/PedidoPersonalizado.cs
namespace AgentWorking.Domain.Entities;

public class PedidoPersonalizado : Pedido
{
    public string Especie { get; set; } = string.Empty;
    public decimal QuantidadeTotal { get; set; }
    public DateTime DataLimite { get; set; }
    public string Observacoes { get; set; } = string.Empty;
    public DateTime PrazoAceite { get; set; }
}
```

- [ ] **Step 8: Create ItemPedido**

```csharp
// AgentWorking.Domain/Entities/ItemPedido.cs
namespace AgentWorking.Domain.Entities;

public class ItemPedido
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public Guid LoteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal Preco { get; set; }
    public Pedido Pedido { get; set; } = null!;
    public LoteEstoque Lote { get; set; } = null!;
}
```

- [ ] **Step 9: Create Venda**

```csharp
// AgentWorking.Domain/Entities/Venda.cs
namespace AgentWorking.Domain.Entities;

public class Venda
{
    public Guid Id { get; set; }
    public Guid? CompraId { get; set; }
    public Guid PedidoId { get; set; }
    public string ProdutorId { get; set; } = string.Empty;
    public string CompradorId { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataVenda { get; set; }
    public Pedido Pedido { get; set; } = null!;
}
```

- [ ] **Step 10: Create EntregaStatus**

```csharp
// AgentWorking.Domain/Entities/EntregaStatus.cs
using AgentWorking.Domain.Enums;

namespace AgentWorking.Domain.Entities;

public class EntregaStatus
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public StatusEntrega Status { get; set; }
    public DateTime? TimestampSaiu { get; set; }
    public DateTime? TimestampTransporte { get; set; }
    public DateTime? TimestampEntregue { get; set; }
    public Pedido Pedido { get; set; } = null!;
}
```

- [ ] **Step 11: Create Notificacao**

```csharp
// AgentWorking.Domain/Entities/Notificacao.cs
using AgentWorking.Domain.Enums;

namespace AgentWorking.Domain.Entities;

public class Notificacao
{
    public Guid Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public TipoNotificacao Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public bool Lida { get; set; }
    public DateTime Timestamp { get; set; }
}
```

- [ ] **Step 12: Verify build**

```bash
cd /root/api-agent-working && dotnet build AgentWorking.Domain
```

Expected: `Build succeeded.`

- [ ] **Step 13: Commit**

```bash
git -C /root/api-agent-working add AgentWorking.Domain/
git -C /root/api-agent-working commit -m "feat(domain): add all domain entities and IRepository interface"
```

---

## Task 4: EF Core DbContext + entity configurations

**Files:**
- Create: `AgentWorking.Infrastructure/Persistence/AppDbContext.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Configurations/CentroDistribuicaoConfiguration.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Configurations/ProdutoConfiguration.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Configurations/LoteEstoqueConfiguration.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Configurations/CompraConfiguration.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Configurations/PedidoConfiguration.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Configurations/ItemPedidoConfiguration.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Configurations/VendaConfiguration.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Configurations/EntregaStatusConfiguration.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Configurations/NotificacaoConfiguration.cs`

- [ ] **Step 1: Create AppDbContext**

```csharp
// AgentWorking.Infrastructure/Persistence/AppDbContext.cs
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CentroDistribuicao> Centros => Set<CentroDistribuicao>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<LoteEstoque> Lotes => Set<LoteEstoque>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoPersonalizado> PedidosPersonalizados => Set<PedidoPersonalizado>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<EntregaStatus> Entregas => Set<EntregaStatus>();
    public DbSet<Notificacao> Notificacoes => Set<Notificacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
```

- [ ] **Step 2: Create CentroDistribuicaoConfiguration**

```csharp
// AgentWorking.Infrastructure/Persistence/Configurations/CentroDistribuicaoConfiguration.cs
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class CentroDistribuicaoConfiguration : IEntityTypeConfiguration<CentroDistribuicao>
{
    public void Configure(EntityTypeBuilder<CentroDistribuicao> builder)
    {
        builder.ToTable("Centros");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nome).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Endereco).HasMaxLength(500).IsRequired();
        builder.Property(c => c.Cidade).HasMaxLength(100).IsRequired();
    }
}
```

- [ ] **Step 3: Create ProdutoConfiguration**

```csharp
// AgentWorking.Infrastructure/Persistence/Configurations/ProdutoConfiguration.cs
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nome).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Cidade).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Safra).HasMaxLength(20).IsRequired();
        builder.Property(p => p.ProdutorId).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Foto).HasMaxLength(500);
        builder.Property(p => p.Preco).HasPrecision(10, 2);
        builder.Property(p => p.Quantidade).HasPrecision(10, 2);
        builder.Property(p => p.Categoria).HasConversion<string>();
        builder.Property(p => p.Unidade).HasConversion<string>();
        builder.Property(p => p.Status).HasConversion<string>()
            .HasDefaultValue(StatusOferta.Ativo);
        builder.HasOne(p => p.CentroDistribuicao)
            .WithMany(c => c.Produtos)
            .HasForeignKey(p => p.CentroDistribuicaoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

- [ ] **Step 4: Create LoteEstoqueConfiguration**

```csharp
// AgentWorking.Infrastructure/Persistence/Configurations/LoteEstoqueConfiguration.cs
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class LoteEstoqueConfiguration : IEntityTypeConfiguration<LoteEstoque>
{
    public void Configure(EntityTypeBuilder<LoteEstoque> builder)
    {
        builder.ToTable("Lotes");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Nome).HasMaxLength(200).IsRequired();
        builder.Property(l => l.CompradorId).HasMaxLength(50).IsRequired();
        builder.Property(l => l.Quantidade).HasPrecision(10, 2);
        builder.Property(l => l.PrecoVenda).HasPrecision(10, 2);
        builder.Property(l => l.Validade).IsRequired();
        builder.HasOne(l => l.Compra)
            .WithOne(c => c.Lote)
            .HasForeignKey<LoteEstoque>(l => l.CompraId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.Produto)
            .WithMany()
            .HasForeignKey(l => l.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

- [ ] **Step 5: Create CompraConfiguration**

```csharp
// AgentWorking.Infrastructure/Persistence/Configurations/CompraConfiguration.cs
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class CompraConfiguration : IEntityTypeConfiguration<Compra>
{
    public void Configure(EntityTypeBuilder<Compra> builder)
    {
        builder.ToTable("Compras");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.ProdutorId).HasMaxLength(50).IsRequired();
        builder.Property(c => c.CompradorId).HasMaxLength(50).IsRequired();
        builder.Property(c => c.Quantidade).HasPrecision(10, 2);
        builder.Property(c => c.PrecoUnitario).HasPrecision(10, 2);
        builder.HasOne(c => c.Produto)
            .WithMany(p => p.Compras)
            .HasForeignKey(c => c.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

- [ ] **Step 6: Create PedidoConfiguration (TPH)**

```csharp
// AgentWorking.Infrastructure/Persistence/Configurations/PedidoConfiguration.cs
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.ClienteId).HasMaxLength(50).IsRequired();
        builder.Property(p => p.CompradorId).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Endereco).HasMaxLength(500).IsRequired();
        builder.Property(p => p.StatusPagamento).HasMaxLength(20);
        builder.Property(p => p.Status).HasConversion<string>();
        builder.Property(p => p.MetodoPagamento).HasConversion<string>();

        // TPH discriminator
        builder.HasDiscriminator(p => p.Tipo)
            .HasValue<Pedido>(TipoPedido.Padrao)
            .HasValue<PedidoPersonalizado>(TipoPedido.Personalizado);
        builder.Property(p => p.Tipo).HasConversion<string>();

        // PedidoPersonalizado nullable columns (TPH)
        builder.Property<string?>("Especie").HasMaxLength(200);
        builder.Property<decimal?>("QuantidadeTotal").HasPrecision(10, 2);
        builder.Property<string?>("Observacoes").HasMaxLength(1000);
    }
}
```

- [ ] **Step 7: Create ItemPedidoConfiguration**

```csharp
// AgentWorking.Infrastructure/Persistence/Configurations/ItemPedidoConfiguration.cs
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("PedidoItens");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Nome).HasMaxLength(200).IsRequired();
        builder.Property(i => i.Quantidade).HasPrecision(10, 2);
        builder.Property(i => i.Preco).HasPrecision(10, 2);
        builder.HasOne(i => i.Pedido)
            .WithMany(p => p.Itens)
            .HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(i => i.Lote)
            .WithMany(l => l.ItensPedido)
            .HasForeignKey(i => i.LoteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

- [ ] **Step 8: Create VendaConfiguration**

```csharp
// AgentWorking.Infrastructure/Persistence/Configurations/VendaConfiguration.cs
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.ProdutorId).HasMaxLength(50).IsRequired();
        builder.Property(v => v.CompradorId).HasMaxLength(50).IsRequired();
        builder.Property(v => v.Quantidade).HasPrecision(10, 2);
        builder.Property(v => v.ValorTotal).HasPrecision(10, 2);
        builder.HasOne(v => v.Pedido)
            .WithMany(p => p.Vendas)
            .HasForeignKey(v => v.PedidoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

- [ ] **Step 9: Create EntregaStatusConfiguration**

```csharp
// AgentWorking.Infrastructure/Persistence/Configurations/EntregaStatusConfiguration.cs
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class EntregaStatusConfiguration : IEntityTypeConfiguration<EntregaStatus>
{
    public void Configure(EntityTypeBuilder<EntregaStatus> builder)
    {
        builder.ToTable("Entregas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Status).HasConversion<string>();
        builder.HasOne(e => e.Pedido)
            .WithOne(p => p.Entrega)
            .HasForeignKey<EntregaStatus>(e => e.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

- [ ] **Step 10: Create NotificacaoConfiguration**

```csharp
// AgentWorking.Infrastructure/Persistence/Configurations/NotificacaoConfiguration.cs
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentWorking.Infrastructure.Persistence.Configurations;

public class NotificacaoConfiguration : IEntityTypeConfiguration<Notificacao>
{
    public void Configure(EntityTypeBuilder<Notificacao> builder)
    {
        builder.ToTable("Notificacoes");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.UsuarioId).HasMaxLength(50).IsRequired();
        builder.Property(n => n.Titulo).HasMaxLength(200).IsRequired();
        builder.Property(n => n.Mensagem).HasMaxLength(1000).IsRequired();
        builder.Property(n => n.Tipo).HasConversion<string>();
        builder.HasIndex(n => n.UsuarioId);
        builder.HasIndex(n => new { n.UsuarioId, n.Lida });
    }
}
```

- [ ] **Step 11: Verify Infrastructure builds**

```bash
cd /root/api-agent-working && dotnet build AgentWorking.Infrastructure
```

Expected: `Build succeeded.`

- [ ] **Step 12: Commit**

```bash
git -C /root/api-agent-working add AgentWorking.Infrastructure/
git -C /root/api-agent-working commit -m "feat(infra): add AppDbContext and all EF entity configurations"
```

---

## Task 5: Migrations + seed data + connection string

**Files:**
- Modify: `AgentWorking.API/appsettings.Development.json`
- Create: `AgentWorking.Infrastructure/Persistence/SeedData.cs`
- Create: `AgentWorking.Infrastructure/DependencyInjection.cs` (partial — just EF registration for migrations)

- [ ] **Step 1: Add connection string to appsettings.Development.json**

```json
// AgentWorking.API/appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=working_agent;Username=william_admin;Password=postgres"
  }
}
```

Note: `Password=postgres` is for local dev only. Production uses `DB_PASSWORD` env var via docker-compose.

- [ ] **Step 2: Create Infrastructure DependencyInjection (EF registration only for now)**

```csharp
// AgentWorking.Infrastructure/DependencyInjection.cs
using AgentWorking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgentWorking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        return services;
    }
}
```

- [ ] **Step 3: Temporarily register DbContext in Program.cs (for migrations only)**

```csharp
// AgentWorking.API/Program.cs — replace entire file
using AgentWorking.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

- [ ] **Step 4: Start PostgreSQL locally**

```bash
cd /root && docker compose up db -d
```

Expected: `Container postgres  Started`

- [ ] **Step 5: Create initial migration**

```bash
cd /root/api-agent-working
dotnet ef migrations add InitialCreate \
  --project AgentWorking.Infrastructure \
  --startup-project AgentWorking.API \
  --output-dir Persistence/Migrations
```

Expected: `Build succeeded.` + `Done. To undo this action, use 'ef migrations remove'`

- [ ] **Step 6: Apply migration**

```bash
dotnet ef database update \
  --project AgentWorking.Infrastructure \
  --startup-project AgentWorking.API
```

Expected: `Done.`

- [ ] **Step 7: Create SeedData**

```csharp
// AgentWorking.Infrastructure/Persistence/SeedData.cs
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
```

- [ ] **Step 8: Call SeedData in Program.cs after build**

```csharp
// AgentWorking.API/Program.cs — add after var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AgentWorking.Infrastructure.Persistence.AppDbContext>();
    await AgentWorking.Infrastructure.Persistence.SeedData.SeedAsync(db);
}
```

- [ ] **Step 9: Run app and verify no errors**

```bash
cd /root/api-agent-working && dotnet run --project AgentWorking.API &
sleep 3
curl -s http://localhost:5000/swagger/index.html | head -5
kill %1
```

Expected: HTML response from Swagger UI

- [ ] **Step 10: Commit**

```bash
git -C /root/api-agent-working add -A
git -C /root/api-agent-working commit -m "feat(infra): add EF migrations, seed data, connection string"
```

---

## Task 6: Repositories + UnitOfWork

**Files:**
- Create: `AgentWorking.Infrastructure/Persistence/Repositories/Repository.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Repositories/CentroRepository.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Repositories/ProdutoRepository.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Repositories/LoteRepository.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Repositories/CompraRepository.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Repositories/PedidoRepository.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Repositories/VendaRepository.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Repositories/EntregaRepository.cs`
- Create: `AgentWorking.Infrastructure/Persistence/Repositories/NotificacaoRepository.cs`
- Create: `AgentWorking.Infrastructure/Persistence/UnitOfWork.cs`
- Create: `AgentWorking.Application/Interfaces/IUnitOfWork.cs`
- Create: `AgentWorking.Application/Interfaces/ICentroRepository.cs`
- Create: `AgentWorking.Application/Interfaces/IProdutoRepository.cs`
- Create: `AgentWorking.Application/Interfaces/ILoteRepository.cs`
- Create: `AgentWorking.Application/Interfaces/ICompraRepository.cs`
- Create: `AgentWorking.Application/Interfaces/IPedidoRepository.cs`
- Create: `AgentWorking.Application/Interfaces/IVendaRepository.cs`
- Create: `AgentWorking.Application/Interfaces/IEntregaRepository.cs`
- Create: `AgentWorking.Application/Interfaces/INotificacaoRepository.cs`

- [ ] **Step 1: Create Application interfaces**

```csharp
// AgentWorking.Application/Interfaces/IUnitOfWork.cs
namespace AgentWorking.Application.Interfaces;
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

```csharp
// AgentWorking.Application/Interfaces/ICentroRepository.cs
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface ICentroRepository : IRepository<CentroDistribuicao>
{
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
```

```csharp
// AgentWorking.Application/Interfaces/IProdutoRepository.cs
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface IProdutoRepository : IRepository<Produto>
{
    Task<List<Produto>> GetByCentroAsync(Guid centroId, CancellationToken ct = default);
    Task<List<Produto>> GetByProdutorAsync(string produtorId, CancellationToken ct = default);
}
```

```csharp
// AgentWorking.Application/Interfaces/ILoteRepository.cs
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface ILoteRepository : IRepository<LoteEstoque>
{
    Task<List<LoteEstoque>> GetByCompradorAsync(string compradorId, CancellationToken ct = default);
    Task<List<LoteEstoque>> GetCatalogoAsync(string? categoria, CancellationToken ct = default);
}
```

```csharp
// AgentWorking.Application/Interfaces/ICompraRepository.cs
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface ICompraRepository : IRepository<Compra>
{
    Task<List<Compra>> GetByCompradorAsync(string compradorId, CancellationToken ct = default);
}
```

```csharp
// AgentWorking.Application/Interfaces/IPedidoRepository.cs
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface IPedidoRepository : IRepository<Pedido>
{
    Task<List<Pedido>> GetByCompradorAsync(string compradorId, CancellationToken ct = default);
    Task<List<Pedido>> GetByClienteAsync(string clienteId, CancellationToken ct = default);
    Task<List<PedidoPersonalizado>> GetExpiredPersonalizadosAsync(CancellationToken ct = default);
}
```

```csharp
// AgentWorking.Application/Interfaces/IVendaRepository.cs
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface IVendaRepository : IRepository<Venda>
{
    Task<List<Venda>> GetByProdutorAsync(string produtorId, CancellationToken ct = default);
}
```

```csharp
// AgentWorking.Application/Interfaces/IEntregaRepository.cs
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface IEntregaRepository : IRepository<EntregaStatus>
{
    Task<EntregaStatus?> GetByPedidoAsync(Guid pedidoId, CancellationToken ct = default);
}
```

```csharp
// AgentWorking.Application/Interfaces/INotificacaoRepository.cs
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Interfaces;
namespace AgentWorking.Application.Interfaces;
public interface INotificacaoRepository : IRepository<Notificacao>
{
    Task<List<Notificacao>> GetByUsuarioAsync(string usuarioId, CancellationToken ct = default);
    Task MarkAllAsReadAsync(string usuarioId, CancellationToken ct = default);
}
```

- [ ] **Step 2: Create generic Repository base**

```csharp
// AgentWorking.Infrastructure/Persistence/Repositories/Repository.cs
using AgentWorking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : class
{
    protected readonly AppDbContext Context = context;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await Context.Set<T>().FindAsync([id], ct);

    public async Task<List<T>> GetAllAsync(CancellationToken ct = default)
        => await Context.Set<T>().ToListAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await Context.Set<T>().AddAsync(entity, ct);

    public void Update(T entity) => Context.Set<T>().Update(entity);

    public void Remove(T entity) => Context.Set<T>().Remove(entity);
}
```

- [ ] **Step 3: Create concrete repositories**

```csharp
// AgentWorking.Infrastructure/Persistence/Repositories/CentroRepository.cs
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class CentroRepository(AppDbContext context)
    : Repository<CentroDistribuicao>(context), ICentroRepository
{
    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await Context.Centros.AnyAsync(c => c.Id == id, ct);
}
```

```csharp
// AgentWorking.Infrastructure/Persistence/Repositories/ProdutoRepository.cs
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class ProdutoRepository(AppDbContext context)
    : Repository<Produto>(context), IProdutoRepository
{
    public async Task<List<Produto>> GetByCentroAsync(Guid centroId, CancellationToken ct = default)
        => await Context.Produtos
            .Where(p => p.CentroDistribuicaoId == centroId)
            .ToListAsync(ct);

    public async Task<List<Produto>> GetByProdutorAsync(string produtorId, CancellationToken ct = default)
        => await Context.Produtos
            .Where(p => p.ProdutorId == produtorId)
            .ToListAsync(ct);
}
```

```csharp
// AgentWorking.Infrastructure/Persistence/Repositories/LoteRepository.cs
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class LoteRepository(AppDbContext context)
    : Repository<LoteEstoque>(context), ILoteRepository
{
    public async Task<List<LoteEstoque>> GetByCompradorAsync(string compradorId, CancellationToken ct = default)
        => await Context.Lotes
            .Include(l => l.Produto)
            .Where(l => l.CompradorId == compradorId)
            .ToListAsync(ct);

    public async Task<List<LoteEstoque>> GetCatalogoAsync(string? categoria, CancellationToken ct = default)
    {
        var query = Context.Lotes
            .Include(l => l.Produto)
            .Where(l => l.Validade > DateTime.UtcNow && l.Quantidade > 0);

        if (!string.IsNullOrEmpty(categoria)
            && Enum.TryParse<Categoria>(categoria, true, out var cat))
        {
            query = query.Where(l => l.Produto.Categoria == cat);
        }

        return await query.ToListAsync(ct);
    }
}
```

```csharp
// AgentWorking.Infrastructure/Persistence/Repositories/CompraRepository.cs
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class CompraRepository(AppDbContext context)
    : Repository<Compra>(context), ICompraRepository
{
    public async Task<List<Compra>> GetByCompradorAsync(string compradorId, CancellationToken ct = default)
        => await Context.Compras
            .Include(c => c.Produto)
            .Include(c => c.Lote)
            .Where(c => c.CompradorId == compradorId)
            .OrderByDescending(c => c.DataCompra)
            .ToListAsync(ct);
}
```

```csharp
// AgentWorking.Infrastructure/Persistence/Repositories/PedidoRepository.cs
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class PedidoRepository(AppDbContext context)
    : Repository<Pedido>(context), IPedidoRepository
{
    public async Task<List<Pedido>> GetByCompradorAsync(string compradorId, CancellationToken ct = default)
        => await Context.Pedidos
            .Include(p => p.Itens)
            .Where(p => p.CompradorId == compradorId)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync(ct);

    public async Task<List<Pedido>> GetByClienteAsync(string clienteId, CancellationToken ct = default)
        => await Context.Pedidos
            .Include(p => p.Itens)
            .Include(p => p.Entrega)
            .Where(p => p.ClienteId == clienteId)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync(ct);

    public async Task<List<PedidoPersonalizado>> GetExpiredPersonalizadosAsync(CancellationToken ct = default)
        => await Context.PedidosPersonalizados
            .Where(p => p.Status == StatusPedido.Pendente && p.PrazoAceite < DateTime.UtcNow)
            .ToListAsync(ct);
}
```

```csharp
// AgentWorking.Infrastructure/Persistence/Repositories/VendaRepository.cs
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class VendaRepository(AppDbContext context)
    : Repository<Venda>(context), IVendaRepository
{
    public async Task<List<Venda>> GetByProdutorAsync(string produtorId, CancellationToken ct = default)
        => await Context.Vendas
            .Include(v => v.Pedido)
            .Where(v => v.ProdutorId == produtorId)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync(ct);
}
```

```csharp
// AgentWorking.Infrastructure/Persistence/Repositories/EntregaRepository.cs
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class EntregaRepository(AppDbContext context)
    : Repository<EntregaStatus>(context), IEntregaRepository
{
    public async Task<EntregaStatus?> GetByPedidoAsync(Guid pedidoId, CancellationToken ct = default)
        => await Context.Entregas.FirstOrDefaultAsync(e => e.PedidoId == pedidoId, ct);
}
```

```csharp
// AgentWorking.Infrastructure/Persistence/Repositories/NotificacaoRepository.cs
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentWorking.Infrastructure.Persistence.Repositories;

public class NotificacaoRepository(AppDbContext context)
    : Repository<Notificacao>(context), INotificacaoRepository
{
    public async Task<List<Notificacao>> GetByUsuarioAsync(string usuarioId, CancellationToken ct = default)
        => await Context.Notificacoes
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.Timestamp)
            .Take(50)
            .ToListAsync(ct);

    public async Task MarkAllAsReadAsync(string usuarioId, CancellationToken ct = default)
        => await Context.Notificacoes
            .Where(n => n.UsuarioId == usuarioId && !n.Lida)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.Lida, true), ct);
}
```

- [ ] **Step 4: Create UnitOfWork**

```csharp
// AgentWorking.Infrastructure/Persistence/UnitOfWork.cs
using AgentWorking.Application.Interfaces;

namespace AgentWorking.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await context.SaveChangesAsync(ct);
}
```

- [ ] **Step 5: Verify build**

```bash
cd /root/api-agent-working && dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 6: Commit**

```bash
git -C /root/api-agent-working add -A
git -C /root/api-agent-working commit -m "feat(infra): add repositories, UnitOfWork, and Application interfaces"
```

---

## Task 7: Application layer foundation (DTOs + ValidationBehavior + DI)

**Files:**
- Create: `AgentWorking.Application/DTOs/*.cs` (all DTO files)
- Create: `AgentWorking.Application/Behaviors/ValidationBehavior.cs`
- Create: `AgentWorking.Application/DependencyInjection.cs`

- [ ] **Step 1: Create DTOs**

```csharp
// AgentWorking.Application/DTOs/CentroDto.cs
namespace AgentWorking.Application.DTOs;
public record CentroDto(Guid Id, string Nome, string Endereco, string Cidade);
```

```csharp
// AgentWorking.Application/DTOs/ProdutoDto.cs
namespace AgentWorking.Application.DTOs;
public record ProdutoDto(
    Guid Id, string Nome, string Categoria, decimal Quantidade,
    string Unidade, decimal Preco, string Safra, string Cidade,
    string? Foto, string ProdutorId, string Status,
    Guid CentroDistribuicaoId);
```

```csharp
// AgentWorking.Application/DTOs/LoteDto.cs
namespace AgentWorking.Application.DTOs;
public record LoteDto(
    Guid Id, Guid CompraId, Guid ProdutoId, string Nome,
    decimal Quantidade, DateTime Validade, decimal PrecoVenda,
    string CompradorId);
```

```csharp
// AgentWorking.Application/DTOs/CompraDto.cs
namespace AgentWorking.Application.DTOs;
public record CompraDto(
    Guid Id, string ProdutorId, string CompradorId, Guid ProdutoId,
    string ProdutoNome, decimal Quantidade, decimal PrecoUnitario,
    DateTime DataCompra, Guid? LoteId);
```

```csharp
// AgentWorking.Application/DTOs/PedidoDto.cs
namespace AgentWorking.Application.DTOs;
public record ItemPedidoDto(Guid Id, Guid LoteId, string Nome, decimal Quantidade, decimal Preco);
public record PedidoDto(
    Guid Id, string Tipo, string ClienteId, string CompradorId,
    string Endereco, string Status, DateTime DataCriacao,
    DateTime? DataEntrega, string? MetodoPagamento, string StatusPagamento,
    List<ItemPedidoDto> Itens,
    // personalizado fields (null for padrao)
    string? Especie, decimal? QuantidadeTotal, DateTime? DataLimite,
    string? Observacoes, DateTime? PrazoAceite);
```

```csharp
// AgentWorking.Application/DTOs/VendaDto.cs
namespace AgentWorking.Application.DTOs;
public record VendaDto(
    Guid Id, Guid? CompraId, Guid PedidoId, string ProdutorId,
    string CompradorId, decimal Quantidade, decimal ValorTotal,
    DateTime DataVenda);
```

```csharp
// AgentWorking.Application/DTOs/EntregaDto.cs
namespace AgentWorking.Application.DTOs;
public record EntregaDto(
    Guid Id, Guid PedidoId, string Status,
    DateTime? TimestampSaiu, DateTime? TimestampTransporte,
    DateTime? TimestampEntregue);
```

```csharp
// AgentWorking.Application/DTOs/NotificacaoDto.cs
namespace AgentWorking.Application.DTOs;
public record NotificacaoDto(
    Guid Id, string UsuarioId, string Tipo, string Titulo,
    string Mensagem, bool Lida, DateTime Timestamp);
```

```csharp
// AgentWorking.Application/DTOs/PagamentoDto.cs
namespace AgentWorking.Application.DTOs;
public record PagamentoPixDto(string QrCode, string ChavePix, decimal Valor);
public record PagamentoCartaoDto(bool Aprovado, string CodigoAutorizacao);
public record PagamentoBoletoDto(string CodigoBarras, string LinhaDigitavel, string PdfUrl);
```

- [ ] **Step 2: Create ValidationBehavior**

```csharp
// AgentWorking.Application/Behaviors/ValidationBehavior.cs
using FluentValidation;
using MediatR;

namespace AgentWorking.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(e => e != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
```

- [ ] **Step 3: Create Application DependencyInjection**

```csharp
// AgentWorking.Application/DependencyInjection.cs
using AgentWorking.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AgentWorking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        return services;
    }
}
```

- [ ] **Step 4: Update Infrastructure DependencyInjection to register repositories**

```csharp
// AgentWorking.Infrastructure/DependencyInjection.cs — replace entire file
using AgentWorking.Application.Interfaces;
using AgentWorking.Infrastructure.Persistence;
using AgentWorking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgentWorking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICentroRepository, CentroRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<ILoteRepository, LoteRepository>();
        services.AddScoped<ICompraRepository, CompraRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IVendaRepository, VendaRepository>();
        services.AddScoped<IEntregaRepository, EntregaRepository>();
        services.AddScoped<INotificacaoRepository, NotificacaoRepository>();

        return services;
    }
}
```

- [ ] **Step 5: Verify build**

```bash
cd /root/api-agent-working && dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 6: Commit**

```bash
git -C /root/api-agent-working add -A
git -C /root/api-agent-working commit -m "feat(app): add DTOs, ValidationBehavior, and DI extensions"
```

---

## Task 8: Centros + Produtos features (with tests)

**Files:**
- Create: `AgentWorking.Application/Features/Centros/Queries/GetCentros/GetCentrosQuery.cs`
- Create: `AgentWorking.Application/Features/Centros/Queries/GetCentros/GetCentrosHandler.cs`
- Create: `AgentWorking.Application/Features/Produtos/Queries/GetProdutos/GetProdutosQuery.cs`
- Create: `AgentWorking.Application/Features/Produtos/Queries/GetProdutos/GetProdutosHandler.cs`
- Create: `AgentWorking.Application/Features/Produtos/Commands/CreateProduto/CreateProdutoCommand.cs`
- Create: `AgentWorking.Application/Features/Produtos/Commands/CreateProduto/CreateProdutoHandler.cs`
- Create: `AgentWorking.Application/Features/Produtos/Commands/CreateProduto/CreateProdutoValidator.cs`
- Create: `AgentWorking.Application/Features/Produtos/Commands/UpdateProduto/UpdateProdutoCommand.cs`
- Create: `AgentWorking.Application/Features/Produtos/Commands/UpdateProduto/UpdateProdutoHandler.cs`
- Create: `AgentWorking.Application/Features/Produtos/Commands/UpdateProduto/UpdateProdutoValidator.cs`
- Create: `AgentWorking.Application/Features/Produtos/Commands/PatchProdutoStatus/PatchProdutoStatusCommand.cs`
- Create: `AgentWorking.Application/Features/Produtos/Commands/PatchProdutoStatus/PatchProdutoStatusHandler.cs`
- Test: `AgentWorking.Tests/Application/Features/Produtos/CreateProdutoHandlerTests.cs`

- [ ] **Step 1: Write the failing test first**

```csharp
// AgentWorking.Tests/Application/Features/Produtos/CreateProdutoHandlerTests.cs
using AgentWorking.Application.Features.Produtos.Commands.CreateProduto;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Moq;

namespace AgentWorking.Tests.Application.Features.Produtos;

public class CreateProdutoHandlerTests
{
    private readonly Mock<IProdutoRepository> _repoMock = new();
    private readonly Mock<ICentroRepository> _centroMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    [Fact]
    public async Task Handle_ValidCommand_CreatesProdutoWithStatusAtivo()
    {
        var centroId = Guid.NewGuid();
        _centroMock.Setup(r => r.ExistsAsync(centroId, default)).ReturnsAsync(true);

        Produto? captured = null;
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Produto>(), default))
            .Callback<Produto, CancellationToken>((p, _) => captured = p)
            .Returns(Task.CompletedTask);

        var handler = new CreateProdutoHandler(_repoMock.Object, _centroMock.Object, _uowMock.Object);

        var result = await handler.Handle(new CreateProdutoCommand(
            Nome: "Tomate Carmem",
            Categoria: Categoria.Verduras,
            Quantidade: 150,
            Unidade: UnidadeMedida.Kg,
            Preco: 4.5m,
            Safra: "2026-05",
            Cidade: "Campinas",
            ProdutorId: "prod-1",
            CentroDistribuicaoId: centroId,
            Foto: null), default);

        Assert.NotNull(captured);
        Assert.Equal("Tomate Carmem", captured.Nome);
        Assert.Equal(StatusOferta.Ativo, captured.Status);
        Assert.Equal(centroId, captured.CentroDistribuicaoId);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCentro_ThrowsKeyNotFoundException()
    {
        _centroMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), default)).ReturnsAsync(false);
        var handler = new CreateProdutoHandler(_repoMock.Object, _centroMock.Object, _uowMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new CreateProdutoCommand(
                "X", Categoria.Frutas, 1, UnidadeMedida.Kg, 1m,
                "2026-05", "SP", "prod-1", Guid.NewGuid(), null), default));
    }
}
```

- [ ] **Step 2: Run test — expect FAIL (types not defined yet)**

```bash
cd /root/api-agent-working && dotnet test AgentWorking.Tests --filter "CreateProdutoHandlerTests" 2>&1 | tail -5
```

Expected: compile error — `CreateProdutoCommand` not found

- [ ] **Step 3: Create GetCentros feature**

```csharp
// AgentWorking.Application/Features/Centros/Queries/GetCentros/GetCentrosQuery.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Centros.Queries.GetCentros;
public record GetCentrosQuery : IRequest<List<CentroDto>>;
```

```csharp
// AgentWorking.Application/Features/Centros/Queries/GetCentros/GetCentrosHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Centros.Queries.GetCentros;

public class GetCentrosHandler(ICentroRepository repo) : IRequestHandler<GetCentrosQuery, List<CentroDto>>
{
    public async Task<List<CentroDto>> Handle(GetCentrosQuery request, CancellationToken ct)
    {
        var centros = await repo.GetAllAsync(ct);
        return centros.Select(c => new CentroDto(c.Id, c.Nome, c.Endereco, c.Cidade)).ToList();
    }
}
```

- [ ] **Step 4: Create GetProdutos feature**

```csharp
// AgentWorking.Application/Features/Produtos/Queries/GetProdutos/GetProdutosQuery.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Queries.GetProdutos;
public record GetProdutosQuery(Guid? CentroId, string? ProdutorId) : IRequest<List<ProdutoDto>>;
```

```csharp
// AgentWorking.Application/Features/Produtos/Queries/GetProdutos/GetProdutosHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Queries.GetProdutos;

public class GetProdutosHandler(IProdutoRepository repo) : IRequestHandler<GetProdutosQuery, List<ProdutoDto>>
{
    public async Task<List<ProdutoDto>> Handle(GetProdutosQuery request, CancellationToken ct)
    {
        List<Produto> produtos;
        if (request.CentroId.HasValue)
            produtos = await repo.GetByCentroAsync(request.CentroId.Value, ct);
        else if (!string.IsNullOrEmpty(request.ProdutorId))
            produtos = await repo.GetByProdutorAsync(request.ProdutorId, ct);
        else
            produtos = await repo.GetAllAsync(ct);

        return produtos.Select(ToDto).ToList();
    }

    private static ProdutoDto ToDto(Produto p) => new(
        p.Id, p.Nome, p.Categoria.ToString(), p.Quantidade, p.Unidade.ToString(),
        p.Preco, p.Safra, p.Cidade, p.Foto, p.ProdutorId, p.Status.ToString(),
        p.CentroDistribuicaoId);
}
```

- [ ] **Step 5: Create CreateProduto command + handler + validator**

```csharp
// AgentWorking.Application/Features/Produtos/Commands/CreateProduto/CreateProdutoCommand.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Commands.CreateProduto;
public record CreateProdutoCommand(
    string Nome, Categoria Categoria, decimal Quantidade, UnidadeMedida Unidade,
    decimal Preco, string Safra, string Cidade, string ProdutorId,
    Guid CentroDistribuicaoId, string? Foto) : IRequest<ProdutoDto>;
```

```csharp
// AgentWorking.Application/Features/Produtos/Commands/CreateProduto/CreateProdutoHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Commands.CreateProduto;

public class CreateProdutoHandler(
    IProdutoRepository repo,
    ICentroRepository centroRepo,
    IUnitOfWork uow) : IRequestHandler<CreateProdutoCommand, ProdutoDto>
{
    public async Task<ProdutoDto> Handle(CreateProdutoCommand cmd, CancellationToken ct)
    {
        if (!await centroRepo.ExistsAsync(cmd.CentroDistribuicaoId, ct))
            throw new KeyNotFoundException($"Centro {cmd.CentroDistribuicaoId} not found");

        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Nome = cmd.Nome,
            Categoria = cmd.Categoria,
            Quantidade = cmd.Quantidade,
            Unidade = cmd.Unidade,
            Preco = cmd.Preco,
            Safra = cmd.Safra,
            Cidade = cmd.Cidade,
            ProdutorId = cmd.ProdutorId,
            CentroDistribuicaoId = cmd.CentroDistribuicaoId,
            Foto = cmd.Foto,
            Status = Domain.Enums.StatusOferta.Ativo
        };

        await repo.AddAsync(produto, ct);
        await uow.SaveChangesAsync(ct);

        return new ProdutoDto(produto.Id, produto.Nome, produto.Categoria.ToString(),
            produto.Quantidade, produto.Unidade.ToString(), produto.Preco,
            produto.Safra, produto.Cidade, produto.Foto, produto.ProdutorId,
            produto.Status.ToString(), produto.CentroDistribuicaoId);
    }
}
```

```csharp
// AgentWorking.Application/Features/Produtos/Commands/CreateProduto/CreateProdutoValidator.cs
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
```

- [ ] **Step 6: Create UpdateProduto + PatchProdutoStatus**

```csharp
// AgentWorking.Application/Features/Produtos/Commands/UpdateProduto/UpdateProdutoCommand.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Commands.UpdateProduto;
public record UpdateProdutoCommand(
    Guid Id, string Nome, Categoria Categoria, decimal Quantidade,
    UnidadeMedida Unidade, decimal Preco, string Safra, string Cidade,
    string? Foto) : IRequest<ProdutoDto>;
```

```csharp
// AgentWorking.Application/Features/Produtos/Commands/UpdateProduto/UpdateProdutoHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Commands.UpdateProduto;

public class UpdateProdutoHandler(IProdutoRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateProdutoCommand, ProdutoDto>
{
    public async Task<ProdutoDto> Handle(UpdateProdutoCommand cmd, CancellationToken ct)
    {
        var produto = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Produto {cmd.Id} not found");

        produto.Nome = cmd.Nome;
        produto.Categoria = cmd.Categoria;
        produto.Quantidade = cmd.Quantidade;
        produto.Unidade = cmd.Unidade;
        produto.Preco = cmd.Preco;
        produto.Safra = cmd.Safra;
        produto.Cidade = cmd.Cidade;
        produto.Foto = cmd.Foto;

        repo.Update(produto);
        await uow.SaveChangesAsync(ct);

        return new ProdutoDto(produto.Id, produto.Nome, produto.Categoria.ToString(),
            produto.Quantidade, produto.Unidade.ToString(), produto.Preco, produto.Safra,
            produto.Cidade, produto.Foto, produto.ProdutorId, produto.Status.ToString(),
            produto.CentroDistribuicaoId);
    }
}
```

```csharp
// AgentWorking.Application/Features/Produtos/Commands/UpdateProduto/UpdateProdutoValidator.cs
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
```

```csharp
// AgentWorking.Application/Features/Produtos/Commands/PatchProdutoStatus/PatchProdutoStatusCommand.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Commands.PatchProdutoStatus;
public record PatchProdutoStatusCommand(Guid Id) : IRequest<ProdutoDto>;
```

```csharp
// AgentWorking.Application/Features/Produtos/Commands/PatchProdutoStatus/PatchProdutoStatusHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Produtos.Commands.PatchProdutoStatus;

public class PatchProdutoStatusHandler(IProdutoRepository repo, IUnitOfWork uow)
    : IRequestHandler<PatchProdutoStatusCommand, ProdutoDto>
{
    public async Task<ProdutoDto> Handle(PatchProdutoStatusCommand cmd, CancellationToken ct)
    {
        var produto = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Produto {cmd.Id} not found");

        produto.Status = produto.Status switch
        {
            StatusOferta.Ativo => StatusOferta.Pausado,
            StatusOferta.Pausado => StatusOferta.Ativo,
            _ => produto.Status
        };

        repo.Update(produto);
        await uow.SaveChangesAsync(ct);

        return new ProdutoDto(produto.Id, produto.Nome, produto.Categoria.ToString(),
            produto.Quantidade, produto.Unidade.ToString(), produto.Preco, produto.Safra,
            produto.Cidade, produto.Foto, produto.ProdutorId, produto.Status.ToString(),
            produto.CentroDistribuicaoId);
    }
}
```

- [ ] **Step 7: Run tests — expect PASS**

```bash
cd /root/api-agent-working && dotnet test AgentWorking.Tests --filter "CreateProdutoHandlerTests" -v
```

Expected: `Passed! - Failed: 0, Passed: 2`

- [ ] **Step 8: Commit**

```bash
git -C /root/api-agent-working add -A
git -C /root/api-agent-working commit -m "feat(app): add Centros and Produtos features with tests"
```

---

## Task 9: Compras + Lotes features (with tests)

**Files:**
- Create: `AgentWorking.Application/Features/Compras/Commands/CreateCompra/CreateCompraCommand.cs` + Handler + Validator
- Create: `AgentWorking.Application/Features/Compras/Queries/GetCompras/GetComprasQuery.cs` + Handler
- Create: `AgentWorking.Application/Features/Lotes/Queries/GetLotes/GetLotesQuery.cs` + Handler
- Create: `AgentWorking.Application/Features/Lotes/Queries/GetCatalogo/GetCatalogoQuery.cs` + Handler
- Create: `AgentWorking.Application/Features/Lotes/Commands/UpdateLote/UpdateLoteCommand.cs` + Handler + Validator
- Test: `AgentWorking.Tests/Application/Features/Compras/CreateCompraHandlerTests.cs`
- Test: `AgentWorking.Tests/Application/Features/Lotes/GetCatalogoHandlerTests.cs`

- [ ] **Step 1: Write failing tests**

```csharp
// AgentWorking.Tests/Application/Features/Compras/CreateCompraHandlerTests.cs
using AgentWorking.Application.Features.Compras.Commands.CreateCompra;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Moq;

namespace AgentWorking.Tests.Application.Features.Compras;

public class CreateCompraHandlerTests
{
    private readonly Mock<IProdutoRepository> _produtoMock = new();
    private readonly Mock<ICompraRepository> _compraMock = new();
    private readonly Mock<ILoteRepository> _loteMock = new();
    private readonly Mock<INotificacaoRepository> _notifMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    [Fact]
    public async Task Handle_ValidCompra_CreatesCompraAndLote()
    {
        var produtoId = Guid.NewGuid();
        var produto = new Produto
        {
            Id = produtoId, Nome = "Tomate", Quantidade = 150,
            Preco = 4.5m, ProdutorId = "prod-1",
            Categoria = Categoria.Verduras, Unidade = UnidadeMedida.Kg,
            Safra = "2026-05", Cidade = "SP", Status = StatusOferta.Ativo,
            CentroDistribuicaoId = Guid.NewGuid()
        };
        _produtoMock.Setup(r => r.GetByIdAsync(produtoId, default)).ReturnsAsync(produto);

        LoteEstoque? capturedLote = null;
        _loteMock.Setup(r => r.AddAsync(It.IsAny<LoteEstoque>(), default))
            .Callback<LoteEstoque, CancellationToken>((l, _) => capturedLote = l)
            .Returns(Task.CompletedTask);

        var handler = new CreateCompraHandler(
            _produtoMock.Object, _compraMock.Object,
            _loteMock.Object, _notifMock.Object, _uowMock.Object);

        var validade = DateTime.UtcNow.AddDays(30);
        var result = await handler.Handle(new CreateCompraCommand(
            ProdutorId: "prod-1", CompradorId: "comp-1",
            ProdutoId: produtoId, Quantidade: 50,
            PrecoUnitario: 4.5m, PrecoVenda: 6.0m,
            Validade: validade), default);

        Assert.NotNull(capturedLote);
        Assert.Equal(50, capturedLote.Quantidade);
        Assert.Equal(validade, capturedLote.Validade);
        Assert.Equal(6.0m, capturedLote.PrecoVenda);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_InsufficientStock_ThrowsInvalidOperationException()
    {
        var produtoId = Guid.NewGuid();
        _produtoMock.Setup(r => r.GetByIdAsync(produtoId, default)).ReturnsAsync(
            new Produto { Id = produtoId, Quantidade = 10, ProdutorId = "prod-1",
                Nome = "X", Safra = "2026", Cidade = "SP",
                Categoria = Categoria.Frutas, Unidade = UnidadeMedida.Kg });

        var handler = new CreateCompraHandler(
            _produtoMock.Object, _compraMock.Object,
            _loteMock.Object, _notifMock.Object, _uowMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new CreateCompraCommand(
                "prod-1", "comp-1", produtoId, 50, 4.5m, 6m,
                DateTime.UtcNow.AddDays(30)), default));
    }
}
```

```csharp
// AgentWorking.Tests/Application/Features/Lotes/GetCatalogoHandlerTests.cs
using AgentWorking.Application.Features.Lotes.Queries.GetCatalogo;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Moq;

namespace AgentWorking.Tests.Application.Features.Lotes;

public class GetCatalogoHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsMappedLoteDtos()
    {
        var mock = new Mock<ILoteRepository>();
        var lotes = new List<LoteEstoque>
        {
            new() {
                Id = Guid.NewGuid(), CompraId = Guid.NewGuid(),
                ProdutoId = Guid.NewGuid(), Nome = "Tomate",
                Quantidade = 80, Validade = DateTime.UtcNow.AddDays(10),
                PrecoVenda = 5.8m, CompradorId = "comp-1"
            }
        };
        mock.Setup(r => r.GetCatalogoAsync(null, default)).ReturnsAsync(lotes);

        var handler = new GetCatalogoHandler(mock.Object);
        var result = await handler.Handle(new GetCatalogoQuery(null), default);

        Assert.Single(result);
        Assert.Equal("Tomate", result[0].Nome);
    }
}
```

- [ ] **Step 2: Run tests — expect FAIL**

```bash
cd /root/api-agent-working && dotnet test AgentWorking.Tests 2>&1 | grep -E "error|FAIL" | head -5
```

Expected: compile errors

- [ ] **Step 3: Implement CreateCompra**

```csharp
// AgentWorking.Application/Features/Compras/Commands/CreateCompra/CreateCompraCommand.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Compras.Commands.CreateCompra;
public record CreateCompraCommand(
    string ProdutorId, string CompradorId, Guid ProdutoId,
    decimal Quantidade, decimal PrecoUnitario, decimal PrecoVenda,
    DateTime Validade) : IRequest<CompraDto>;
```

```csharp
// AgentWorking.Application/Features/Compras/Commands/CreateCompra/CreateCompraHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Compras.Commands.CreateCompra;

public class CreateCompraHandler(
    IProdutoRepository produtoRepo,
    ICompraRepository compraRepo,
    ILoteRepository loteRepo,
    INotificacaoRepository notifRepo,
    IUnitOfWork uow) : IRequestHandler<CreateCompraCommand, CompraDto>
{
    public async Task<CompraDto> Handle(CreateCompraCommand cmd, CancellationToken ct)
    {
        var produto = await produtoRepo.GetByIdAsync(cmd.ProdutoId, ct)
            ?? throw new KeyNotFoundException($"Produto {cmd.ProdutoId} not found");

        if (produto.Quantidade < cmd.Quantidade)
            throw new InvalidOperationException(
                $"Estoque insuficiente. Disponível: {produto.Quantidade}");

        produto.Quantidade -= cmd.Quantidade;
        produtoRepo.Update(produto);

        var compra = new Compra
        {
            Id = Guid.NewGuid(),
            ProdutorId = cmd.ProdutorId,
            CompradorId = cmd.CompradorId,
            ProdutoId = cmd.ProdutoId,
            Quantidade = cmd.Quantidade,
            PrecoUnitario = cmd.PrecoUnitario,
            DataCompra = DateTime.UtcNow
        };
        await compraRepo.AddAsync(compra, ct);

        var lote = new LoteEstoque
        {
            Id = Guid.NewGuid(),
            CompraId = compra.Id,
            ProdutoId = cmd.ProdutoId,
            Nome = produto.Nome,
            Quantidade = cmd.Quantidade,
            Validade = cmd.Validade,
            PrecoVenda = cmd.PrecoVenda,
            CompradorId = cmd.CompradorId
        };
        await loteRepo.AddAsync(lote, ct);

        var notif = new Notificacao
        {
            Id = Guid.NewGuid(),
            UsuarioId = produto.ProdutorId,
            Tipo = TipoNotificacao.Pedido,
            Titulo = "Produto comprado",
            Mensagem = $"Comprador {cmd.CompradorId} comprou {cmd.Quantidade} de {produto.Nome}",
            Lida = false,
            Timestamp = DateTime.UtcNow
        };
        await notifRepo.AddAsync(notif, ct);

        await uow.SaveChangesAsync(ct);

        return new CompraDto(compra.Id, compra.ProdutorId, compra.CompradorId,
            compra.ProdutoId, produto.Nome, compra.Quantidade,
            compra.PrecoUnitario, compra.DataCompra, lote.Id);
    }
}
```

```csharp
// AgentWorking.Application/Features/Compras/Commands/CreateCompra/CreateCompraValidator.cs
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
```

```csharp
// AgentWorking.Application/Features/Compras/Queries/GetCompras/GetComprasQuery.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Compras.Queries.GetCompras;
public record GetComprasQuery(string CompradorId) : IRequest<List<CompraDto>>;
```

```csharp
// AgentWorking.Application/Features/Compras/Queries/GetCompras/GetComprasHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Compras.Queries.GetCompras;

public class GetComprasHandler(ICompraRepository repo) : IRequestHandler<GetComprasQuery, List<CompraDto>>
{
    public async Task<List<CompraDto>> Handle(GetComprasQuery request, CancellationToken ct)
    {
        var compras = await repo.GetByCompradorAsync(request.CompradorId, ct);
        return compras.Select(c => new CompraDto(
            c.Id, c.ProdutorId, c.CompradorId, c.ProdutoId,
            c.Produto?.Nome ?? string.Empty, c.Quantidade,
            c.PrecoUnitario, c.DataCompra, c.Lote?.Id)).ToList();
    }
}
```

- [ ] **Step 4: Implement Lotes queries**

```csharp
// AgentWorking.Application/Features/Lotes/Queries/GetLotes/GetLotesQuery.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Lotes.Queries.GetLotes;
public record GetLotesQuery(string CompradorId) : IRequest<List<LoteDto>>;
```

```csharp
// AgentWorking.Application/Features/Lotes/Queries/GetLotes/GetLotesHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Lotes.Queries.GetLotes;

public class GetLotesHandler(ILoteRepository repo) : IRequestHandler<GetLotesQuery, List<LoteDto>>
{
    public async Task<List<LoteDto>> Handle(GetLotesQuery request, CancellationToken ct)
    {
        var lotes = await repo.GetByCompradorAsync(request.CompradorId, ct);
        return lotes.Select(l => new LoteDto(
            l.Id, l.CompraId, l.ProdutoId, l.Nome,
            l.Quantidade, l.Validade, l.PrecoVenda, l.CompradorId)).ToList();
    }
}
```

```csharp
// AgentWorking.Application/Features/Lotes/Queries/GetCatalogo/GetCatalogoQuery.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Lotes.Queries.GetCatalogo;
public record GetCatalogoQuery(string? Categoria) : IRequest<List<LoteDto>>;
```

```csharp
// AgentWorking.Application/Features/Lotes/Queries/GetCatalogo/GetCatalogoHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Lotes.Queries.GetCatalogo;

public class GetCatalogoHandler(ILoteRepository repo) : IRequestHandler<GetCatalogoQuery, List<LoteDto>>
{
    public async Task<List<LoteDto>> Handle(GetCatalogoQuery request, CancellationToken ct)
    {
        var lotes = await repo.GetCatalogoAsync(request.Categoria, ct);
        return lotes.Select(l => new LoteDto(
            l.Id, l.CompraId, l.ProdutoId, l.Nome,
            l.Quantidade, l.Validade, l.PrecoVenda, l.CompradorId)).ToList();
    }
}
```

```csharp
// AgentWorking.Application/Features/Lotes/Commands/UpdateLote/UpdateLoteCommand.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Lotes.Commands.UpdateLote;
public record UpdateLoteCommand(Guid Id, decimal Quantidade, decimal PrecoVenda, DateTime Validade)
    : IRequest<LoteDto>;
```

```csharp
// AgentWorking.Application/Features/Lotes/Commands/UpdateLote/UpdateLoteHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Lotes.Commands.UpdateLote;

public class UpdateLoteHandler(ILoteRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateLoteCommand, LoteDto>
{
    public async Task<LoteDto> Handle(UpdateLoteCommand cmd, CancellationToken ct)
    {
        var lote = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Lote {cmd.Id} not found");

        lote.Quantidade = cmd.Quantidade;
        lote.PrecoVenda = cmd.PrecoVenda;
        lote.Validade = cmd.Validade;
        repo.Update(lote);
        await uow.SaveChangesAsync(ct);

        return new LoteDto(lote.Id, lote.CompraId, lote.ProdutoId, lote.Nome,
            lote.Quantidade, lote.Validade, lote.PrecoVenda, lote.CompradorId);
    }
}
```

```csharp
// AgentWorking.Application/Features/Lotes/Commands/UpdateLote/UpdateLoteValidator.cs
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
```

- [ ] **Step 5: Run tests — expect PASS**

```bash
cd /root/api-agent-working && dotnet test AgentWorking.Tests -v 2>&1 | tail -10
```

Expected: `Passed! - Failed: 0, Passed: 4`

- [ ] **Step 6: Commit**

```bash
git -C /root/api-agent-working add -A
git -C /root/api-agent-working commit -m "feat(app): add Compras and Lotes features with tests"
```

---

## Task 10: Pedido padrão (transactional checkout) with tests

**Files:**
- Create: `AgentWorking.Application/Features/Pedidos/Commands/CreatePedido/CreatePedidoCommand.cs` + Handler + Validator
- Create: `AgentWorking.Application/Features/Pedidos/Queries/GetPedidos/GetPedidosQuery.cs` + Handler
- Create: `AgentWorking.Application/Features/Pedidos/Commands/PatchPedidoStatus/PatchPedidoStatusCommand.cs` + Handler
- Test: `AgentWorking.Tests/Application/Features/Pedidos/CreatePedidoHandlerTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// AgentWorking.Tests/Application/Features/Pedidos/CreatePedidoHandlerTests.cs
using AgentWorking.Application.Features.Pedidos.Commands.CreatePedido;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Moq;

namespace AgentWorking.Tests.Application.Features.Pedidos;

public class CreatePedidoHandlerTests
{
    private readonly Mock<IPedidoRepository> _pedidoMock = new();
    private readonly Mock<ILoteRepository> _loteMock = new();
    private readonly Mock<IVendaRepository> _vendaMock = new();
    private readonly Mock<IEntregaRepository> _entregaMock = new();
    private readonly Mock<INotificacaoRepository> _notifMock = new();
    private readonly Mock<ICompraRepository> _compraMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private CreatePedidoHandler BuildHandler() => new(
        _pedidoMock.Object, _loteMock.Object, _vendaMock.Object,
        _entregaMock.Object, _notifMock.Object, _compraMock.Object, _uowMock.Object);

    [Fact]
    public async Task Handle_ValidPedido_DecreasesLoteAndCreatesVenda()
    {
        var loteId = Guid.NewGuid();
        var compraId = Guid.NewGuid();
        var lote = new LoteEstoque
        {
            Id = loteId, CompraId = compraId, ProdutoId = Guid.NewGuid(),
            Nome = "Tomate", Quantidade = 80, Validade = DateTime.UtcNow.AddDays(10),
            PrecoVenda = 5.8m, CompradorId = "comp-1"
        };
        var compra = new Compra
        {
            Id = compraId, ProdutorId = "prod-1", CompradorId = "comp-1",
            ProdutoId = lote.ProdutoId, Quantidade = 80, PrecoUnitario = 4.5m,
            DataCompra = DateTime.UtcNow
        };
        _loteMock.Setup(r => r.GetByIdAsync(loteId, default)).ReturnsAsync(lote);
        _compraMock.Setup(r => r.GetByIdAsync(compraId, default)).ReturnsAsync(compra);

        Venda? capturedVenda = null;
        _vendaMock.Setup(r => r.AddAsync(It.IsAny<Venda>(), default))
            .Callback<Venda, CancellationToken>((v, _) => capturedVenda = v)
            .Returns(Task.CompletedTask);

        var cmd = new CreatePedidoCommand(
            ClienteId: "cli-1", CompradorId: "comp-1",
            Endereco: "Rua X, 123",
            MetodoPagamento: MetodoPagamento.Pix,
            Itens: [new CreatePedidoItemDto(loteId, "Tomate", 30, 5.8m)]);

        var result = await BuildHandler().Handle(cmd, default);

        Assert.Equal(50, lote.Quantidade); // 80 - 30 = 50
        Assert.NotNull(capturedVenda);
        Assert.Equal("prod-1", capturedVenda.ProdutorId);
        Assert.Equal(30 * 5.8m, capturedVenda.ValorTotal);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_InsufficientLoteQty_ThrowsInvalidOperationException()
    {
        var loteId = Guid.NewGuid();
        _loteMock.Setup(r => r.GetByIdAsync(loteId, default)).ReturnsAsync(
            new LoteEstoque
            {
                Id = loteId, CompraId = Guid.NewGuid(), ProdutoId = Guid.NewGuid(),
                Nome = "X", Quantidade = 5, Validade = DateTime.UtcNow.AddDays(5),
                PrecoVenda = 1m, CompradorId = "comp-1"
            });

        var cmd = new CreatePedidoCommand("cli-1", "comp-1", "Rua X",
            MetodoPagamento.Pix, [new CreatePedidoItemDto(loteId, "X", 50, 1m)]);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => BuildHandler().Handle(cmd, default));
    }
}
```

- [ ] **Step 2: Run test — expect FAIL**

```bash
cd /root/api-agent-working && dotnet test AgentWorking.Tests 2>&1 | grep -E "error CS" | head -3
```

- [ ] **Step 3: Implement CreatePedido (transactional)**

```csharp
// AgentWorking.Application/Features/Pedidos/Commands/CreatePedido/CreatePedidoCommand.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.CreatePedido;

public record CreatePedidoItemDto(Guid LoteId, string Nome, decimal Quantidade, decimal Preco);
public record CreatePedidoCommand(
    string ClienteId, string CompradorId, string Endereco,
    MetodoPagamento MetodoPagamento, List<CreatePedidoItemDto> Itens) : IRequest<PedidoDto>;
```

```csharp
// AgentWorking.Application/Features/Pedidos/Commands/CreatePedido/CreatePedidoHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.CreatePedido;

public class CreatePedidoHandler(
    IPedidoRepository pedidoRepo,
    ILoteRepository loteRepo,
    IVendaRepository vendaRepo,
    IEntregaRepository entregaRepo,
    INotificacaoRepository notifRepo,
    ICompraRepository compraRepo,
    IUnitOfWork uow) : IRequestHandler<CreatePedidoCommand, PedidoDto>
{
    public async Task<PedidoDto> Handle(CreatePedidoCommand cmd, CancellationToken ct)
    {
        // Validate all lotes have sufficient stock before mutating anything
        var lotes = new Dictionary<Guid, LoteEstoque>();
        foreach (var item in cmd.Itens)
        {
            var lote = await loteRepo.GetByIdAsync(item.LoteId, ct)
                ?? throw new KeyNotFoundException($"Lote {item.LoteId} not found");
            if (lote.Quantidade < item.Quantidade)
                throw new InvalidOperationException(
                    $"Lote {lote.Nome}: estoque insuficiente. Disponível: {lote.Quantidade}");
            lotes[item.LoteId] = lote;
        }

        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            Tipo = TipoPedido.Padrao,
            ClienteId = cmd.ClienteId,
            CompradorId = cmd.CompradorId,
            Endereco = cmd.Endereco,
            Status = StatusPedido.Confirmado,
            DataCriacao = DateTime.UtcNow,
            MetodoPagamento = cmd.MetodoPagamento,
            StatusPagamento = "pendente"
        };
        await pedidoRepo.AddAsync(pedido, ct);

        foreach (var item in cmd.Itens)
        {
            var lote = lotes[item.LoteId];
            lote.Quantidade -= item.Quantidade;
            loteRepo.Update(lote);

            var itemPedido = new ItemPedido
            {
                Id = Guid.NewGuid(),
                PedidoId = pedido.Id,
                LoteId = item.LoteId,
                Nome = item.Nome,
                Quantidade = item.Quantidade,
                Preco = item.Preco
            };
            pedido.Itens.Add(itemPedido);

            var compra = await compraRepo.GetByIdAsync(lote.CompraId, ct);
            if (compra != null)
            {
                var venda = new Venda
                {
                    Id = Guid.NewGuid(),
                    CompraId = lote.CompraId,
                    PedidoId = pedido.Id,
                    ProdutorId = compra.ProdutorId,
                    CompradorId = cmd.CompradorId,
                    Quantidade = item.Quantidade,
                    ValorTotal = item.Quantidade * item.Preco,
                    DataVenda = DateTime.UtcNow
                };
                await vendaRepo.AddAsync(venda, ct);

                await notifRepo.AddAsync(new Notificacao
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = compra.ProdutorId,
                    Tipo = TipoNotificacao.Pedido,
                    Titulo = "Produto vendido",
                    Mensagem = $"{item.Quantidade} {item.Nome} vendido ao cliente",
                    Lida = false, Timestamp = DateTime.UtcNow
                }, ct);
            }
        }

        var entrega = new EntregaStatus
        {
            Id = Guid.NewGuid(), PedidoId = pedido.Id, Status = StatusEntrega.Saiu
        };
        await entregaRepo.AddAsync(entrega, ct);

        await notifRepo.AddAsync(new Notificacao
        {
            Id = Guid.NewGuid(),
            UsuarioId = cmd.CompradorId,
            Tipo = TipoNotificacao.Pedido,
            Titulo = "Novo pedido recebido",
            Mensagem = $"Pedido de {cmd.ClienteId} confirmado",
            Lida = false, Timestamp = DateTime.UtcNow
        }, ct);

        await uow.SaveChangesAsync(ct);

        return new PedidoDto(pedido.Id, pedido.Tipo.ToString(), pedido.ClienteId,
            pedido.CompradorId, pedido.Endereco, pedido.Status.ToString(),
            pedido.DataCriacao, pedido.DataEntrega,
            pedido.MetodoPagamento?.ToString(), pedido.StatusPagamento,
            pedido.Itens.Select(i => new ItemPedidoDto(i.Id, i.LoteId, i.Nome, i.Quantidade, i.Preco)).ToList(),
            null, null, null, null, null);
    }
}
```

```csharp
// AgentWorking.Application/Features/Pedidos/Commands/CreatePedido/CreatePedidoValidator.cs
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
```

- [ ] **Step 4: Implement GetPedidos + PatchPedidoStatus**

```csharp
// AgentWorking.Application/Features/Pedidos/Queries/GetPedidos/GetPedidosQuery.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Queries.GetPedidos;
public record GetPedidosQuery(string? CompradorId, string? ClienteId) : IRequest<List<PedidoDto>>;
```

```csharp
// AgentWorking.Application/Features/Pedidos/Queries/GetPedidos/GetPedidosHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Queries.GetPedidos;

public class GetPedidosHandler(IPedidoRepository repo) : IRequestHandler<GetPedidosQuery, List<PedidoDto>>
{
    public async Task<List<PedidoDto>> Handle(GetPedidosQuery request, CancellationToken ct)
    {
        var pedidos = !string.IsNullOrEmpty(request.CompradorId)
            ? await repo.GetByCompradorAsync(request.CompradorId, ct)
            : await repo.GetByClienteAsync(request.ClienteId ?? string.Empty, ct);

        return pedidos.Select(ToDto).ToList();
    }

    private static PedidoDto ToDto(Pedido p)
    {
        var pp = p as Domain.Entities.PedidoPersonalizado;
        return new PedidoDto(
            p.Id, p.Tipo.ToString(), p.ClienteId, p.CompradorId, p.Endereco,
            p.Status.ToString(), p.DataCriacao, p.DataEntrega,
            p.MetodoPagamento?.ToString(), p.StatusPagamento,
            p.Itens.Select(i => new ItemPedidoDto(i.Id, i.LoteId, i.Nome, i.Quantidade, i.Preco)).ToList(),
            pp?.Especie, pp?.QuantidadeTotal, pp?.DataLimite, pp?.Observacoes, pp?.PrazoAceite);
    }
}
```

```csharp
// AgentWorking.Application/Features/Pedidos/Commands/PatchPedidoStatus/PatchPedidoStatusCommand.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.PatchPedidoStatus;
public record PatchPedidoStatusCommand(Guid Id, StatusPedido NovoStatus, DateTime? NovaDataEntrega)
    : IRequest<PedidoDto>;
```

```csharp
// AgentWorking.Application/Features/Pedidos/Commands/PatchPedidoStatus/PatchPedidoStatusHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.PatchPedidoStatus;

public class PatchPedidoStatusHandler(IPedidoRepository repo, INotificacaoRepository notifRepo, IUnitOfWork uow)
    : IRequestHandler<PatchPedidoStatusCommand, PedidoDto>
{
    public async Task<PedidoDto> Handle(PatchPedidoStatusCommand cmd, CancellationToken ct)
    {
        var pedido = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Pedido {cmd.Id} not found");

        pedido.Status = cmd.NovoStatus;
        if (cmd.NovaDataEntrega.HasValue) pedido.DataEntrega = cmd.NovaDataEntrega;
        repo.Update(pedido);

        await notifRepo.AddAsync(new Domain.Entities.Notificacao
        {
            Id = Guid.NewGuid(),
            UsuarioId = pedido.ClienteId,
            Tipo = Domain.Enums.TipoNotificacao.Pedido,
            Titulo = $"Pedido {cmd.NovoStatus}",
            Mensagem = $"Seu pedido foi {cmd.NovoStatus.ToString().ToLower()}",
            Lida = false, Timestamp = DateTime.UtcNow
        }, ct);

        await uow.SaveChangesAsync(ct);

        var pp = pedido as Domain.Entities.PedidoPersonalizado;
        return new PedidoDto(pedido.Id, pedido.Tipo.ToString(), pedido.ClienteId,
            pedido.CompradorId, pedido.Endereco, pedido.Status.ToString(),
            pedido.DataCriacao, pedido.DataEntrega, pedido.MetodoPagamento?.ToString(),
            pedido.StatusPagamento,
            pedido.Itens.Select(i => new ItemPedidoDto(i.Id, i.LoteId, i.Nome, i.Quantidade, i.Preco)).ToList(),
            pp?.Especie, pp?.QuantidadeTotal, pp?.DataLimite, pp?.Observacoes, pp?.PrazoAceite);
    }
}
```

- [ ] **Step 5: Run tests — expect PASS**

```bash
cd /root/api-agent-working && dotnet test AgentWorking.Tests -v 2>&1 | tail -5
```

Expected: `Passed! - Failed: 0, Passed: 6`

- [ ] **Step 6: Commit**

```bash
git -C /root/api-agent-working add -A
git -C /root/api-agent-working commit -m "feat(app): add transactional Pedido padrão checkout with tests"
```

---

## Task 11: PedidoPersonalizado + ExpirePedidos (with tests)

**Files:**
- Create: `AgentWorking.Application/Features/Pedidos/Commands/CreatePedidoPersonalizado/CreatePedidoPersonalizadoCommand.cs` + Handler + Validator
- Create: `AgentWorking.Application/Features/Pedidos/Commands/ExpirePedidos/ExpirePedidosCommand.cs` + Handler
- Test: `AgentWorking.Tests/Application/Features/Pedidos/ExpirePedidosHandlerTests.cs`

- [ ] **Step 1: Write failing test**

```csharp
// AgentWorking.Tests/Application/Features/Pedidos/ExpirePedidosHandlerTests.cs
using AgentWorking.Application.Features.Pedidos.Commands.ExpirePedidos;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using Moq;

namespace AgentWorking.Tests.Application.Features.Pedidos;

public class ExpirePedidosHandlerTests
{
    [Fact]
    public async Task Handle_ExpiredPedidos_SetsStatusExpiradoAndNotifiesCliente()
    {
        var pedidoMock = new Mock<IPedidoRepository>();
        var notifMock = new Mock<INotificacaoRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        var expired = new PedidoPersonalizado
        {
            Id = Guid.NewGuid(), ClienteId = "cli-1", CompradorId = "comp-1",
            Endereco = "X", Status = StatusPedido.Pendente,
            DataCriacao = DateTime.UtcNow.AddHours(-25),
            PrazoAceite = DateTime.UtcNow.AddHours(-1),
            Especie = "Abacate", QuantidadeTotal = 500,
            DataLimite = DateTime.UtcNow.AddDays(5),
            Observacoes = string.Empty, Tipo = TipoPedido.Personalizado
        };
        pedidoMock.Setup(r => r.GetExpiredPersonalizadosAsync(default))
            .ReturnsAsync([expired]);

        var handler = new ExpirePedidosHandler(pedidoMock.Object, notifMock.Object, uowMock.Object);
        await handler.Handle(new ExpirePedidosCommand(), default);

        Assert.Equal(StatusPedido.Expirado, expired.Status);
        notifMock.Verify(r => r.AddAsync(
            It.Is<Notificacao>(n => n.UsuarioId == "cli-1" && n.Tipo == TipoNotificacao.Alerta),
            default), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_NoExpiredPedidos_DoesNotSaveChanges()
    {
        var pedidoMock = new Mock<IPedidoRepository>();
        var notifMock = new Mock<INotificacaoRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        pedidoMock.Setup(r => r.GetExpiredPersonalizadosAsync(default))
            .ReturnsAsync([]);

        var handler = new ExpirePedidosHandler(pedidoMock.Object, notifMock.Object, uowMock.Object);
        await handler.Handle(new ExpirePedidosCommand(), default);

        uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }
}
```

- [ ] **Step 2: Run test — expect FAIL**

```bash
cd /root/api-agent-working && dotnet test AgentWorking.Tests 2>&1 | grep "error CS" | head -3
```

- [ ] **Step 3: Implement CreatePedidoPersonalizado**

```csharp
// AgentWorking.Application/Features/Pedidos/Commands/CreatePedidoPersonalizado/CreatePedidoPersonalizadoCommand.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.CreatePedidoPersonalizado;
public record CreatePedidoPersonalizadoCommand(
    string ClienteId, string CompradorId, string Endereco,
    string Especie, decimal QuantidadeTotal, DateTime DataLimite,
    string Observacoes) : IRequest<PedidoDto>;
```

```csharp
// AgentWorking.Application/Features/Pedidos/Commands/CreatePedidoPersonalizado/CreatePedidoPersonalizadoHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.CreatePedidoPersonalizado;

public class CreatePedidoPersonalizadoHandler(
    IPedidoRepository pedidoRepo,
    INotificacaoRepository notifRepo,
    IUnitOfWork uow) : IRequestHandler<CreatePedidoPersonalizadoCommand, PedidoDto>
{
    public async Task<PedidoDto> Handle(CreatePedidoPersonalizadoCommand cmd, CancellationToken ct)
    {
        var agora = DateTime.UtcNow;
        var pedido = new PedidoPersonalizado
        {
            Id = Guid.NewGuid(),
            Tipo = TipoPedido.Personalizado,
            ClienteId = cmd.ClienteId,
            CompradorId = string.Empty,
            Endereco = cmd.Endereco,
            Status = StatusPedido.Pendente,
            DataCriacao = agora,
            PrazoAceite = agora.AddHours(24),
            Especie = cmd.Especie,
            QuantidadeTotal = cmd.QuantidadeTotal,
            DataLimite = cmd.DataLimite,
            Observacoes = cmd.Observacoes,
            StatusPagamento = "pendente"
        };
        await pedidoRepo.AddAsync(pedido, ct);

        // Notify ALL compradores — broadcast
        // In no-auth phase, compradores are identified by known IDs.
        // This list should come from a CompradorRepository in a future auth phase.
        // For now, notify the single known comprador.
        await notifRepo.AddAsync(new Notificacao
        {
            Id = Guid.NewGuid(),
            UsuarioId = "comp-1",
            Tipo = TipoNotificacao.Pedido,
            Titulo = "Novo pedido personalizado",
            Mensagem = $"Solicitação: {cmd.QuantidadeTotal} de {cmd.Especie} até {cmd.DataLimite:dd/MM}",
            Lida = false,
            Timestamp = agora
        }, ct);

        await uow.SaveChangesAsync(ct);

        return new PedidoDto(pedido.Id, pedido.Tipo.ToString(), pedido.ClienteId,
            pedido.CompradorId, pedido.Endereco, pedido.Status.ToString(),
            pedido.DataCriacao, pedido.DataEntrega, null, pedido.StatusPagamento,
            [], pedido.Especie, pedido.QuantidadeTotal, pedido.DataLimite,
            pedido.Observacoes, pedido.PrazoAceite);
    }
}
```

```csharp
// AgentWorking.Application/Features/Pedidos/Commands/CreatePedidoPersonalizado/CreatePedidoPersonalizadoValidator.cs
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
```

- [ ] **Step 4: Implement ExpirePedidos**

```csharp
// AgentWorking.Application/Features/Pedidos/Commands/ExpirePedidos/ExpirePedidosCommand.cs
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.ExpirePedidos;
public record ExpirePedidosCommand : IRequest;
```

```csharp
// AgentWorking.Application/Features/Pedidos/Commands/ExpirePedidos/ExpirePedidosHandler.cs
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Entities;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Pedidos.Commands.ExpirePedidos;

public class ExpirePedidosHandler(
    IPedidoRepository pedidoRepo,
    INotificacaoRepository notifRepo,
    IUnitOfWork uow) : IRequestHandler<ExpirePedidosCommand>
{
    public async Task Handle(ExpirePedidosCommand request, CancellationToken ct)
    {
        var expired = await pedidoRepo.GetExpiredPersonalizadosAsync(ct);
        if (expired.Count == 0) return;

        foreach (var pedido in expired)
        {
            pedido.Status = StatusPedido.Expirado;
            await notifRepo.AddAsync(new Notificacao
            {
                Id = Guid.NewGuid(),
                UsuarioId = pedido.ClienteId,
                Tipo = TipoNotificacao.Alerta,
                Titulo = "Pedido personalizado expirado",
                Mensagem = $"Nenhum comprador aceitou seu pedido de {pedido.Especie} no prazo",
                Lida = false,
                Timestamp = DateTime.UtcNow
            }, ct);
        }

        await uow.SaveChangesAsync(ct);
    }
}
```

- [ ] **Step 5: Run tests — expect PASS**

```bash
cd /root/api-agent-working && dotnet test AgentWorking.Tests -v 2>&1 | tail -5
```

Expected: `Passed! - Failed: 0, Passed: 8`

- [ ] **Step 6: Commit**

```bash
git -C /root/api-agent-working add -A
git -C /root/api-agent-working commit -m "feat(app): add PedidoPersonalizado and ExpirePedidos with tests"
```

---

## Task 12: Vendas + Entregas + Notificações + Pagamentos features

**Files:**
- Create: `AgentWorking.Application/Features/Vendas/Queries/GetVendas/GetVendasQuery.cs` + Handler
- Create: `AgentWorking.Application/Features/Entregas/Queries/GetEntrega/GetEntregaQuery.cs` + Handler
- Create: `AgentWorking.Application/Features/Entregas/Commands/PatchEntregaStatus/PatchEntregaStatusCommand.cs` + Handler
- Create: `AgentWorking.Application/Features/Notificacoes/Queries/GetNotificacoes/GetNotificacoesQuery.cs` + Handler
- Create: `AgentWorking.Application/Features/Notificacoes/Commands/PatchNotificacaoLida/PatchNotificacaoLidaCommand.cs` + Handler
- Create: `AgentWorking.Application/Features/Notificacoes/Commands/PatchTodasLidas/PatchTodasLidasCommand.cs` + Handler
- Create: `AgentWorking.Application/Features/Pagamentos/Commands/ProcessPix/ProcessPixCommand.cs` + Handler
- Create: `AgentWorking.Application/Features/Pagamentos/Commands/ProcessCartao/ProcessCartaoCommand.cs` + Handler + Validator
- Create: `AgentWorking.Application/Features/Pagamentos/Commands/ProcessBoleto/ProcessBoletoCommand.cs` + Handler

- [ ] **Step 1: Vendas**

```csharp
// AgentWorking.Application/Features/Vendas/Queries/GetVendas/GetVendasQuery.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Vendas.Queries.GetVendas;
public record GetVendasQuery(string ProdutorId) : IRequest<List<VendaDto>>;
```

```csharp
// AgentWorking.Application/Features/Vendas/Queries/GetVendas/GetVendasHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Vendas.Queries.GetVendas;

public class GetVendasHandler(IVendaRepository repo) : IRequestHandler<GetVendasQuery, List<VendaDto>>
{
    public async Task<List<VendaDto>> Handle(GetVendasQuery request, CancellationToken ct)
    {
        var vendas = await repo.GetByProdutorAsync(request.ProdutorId, ct);
        return vendas.Select(v => new VendaDto(
            v.Id, v.CompraId, v.PedidoId, v.ProdutorId, v.CompradorId,
            v.Quantidade, v.ValorTotal, v.DataVenda)).ToList();
    }
}
```

- [ ] **Step 2: Entregas**

```csharp
// AgentWorking.Application/Features/Entregas/Queries/GetEntrega/GetEntregaQuery.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Entregas.Queries.GetEntrega;
public record GetEntregaQuery(Guid PedidoId) : IRequest<EntregaDto>;
```

```csharp
// AgentWorking.Application/Features/Entregas/Queries/GetEntrega/GetEntregaHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Entregas.Queries.GetEntrega;

public class GetEntregaHandler(IEntregaRepository repo) : IRequestHandler<GetEntregaQuery, EntregaDto>
{
    public async Task<EntregaDto> Handle(GetEntregaQuery request, CancellationToken ct)
    {
        var e = await repo.GetByPedidoAsync(request.PedidoId, ct)
            ?? throw new KeyNotFoundException($"Entrega for pedido {request.PedidoId} not found");
        return new EntregaDto(e.Id, e.PedidoId, e.Status.ToString(),
            e.TimestampSaiu, e.TimestampTransporte, e.TimestampEntregue);
    }
}
```

```csharp
// AgentWorking.Application/Features/Entregas/Commands/PatchEntregaStatus/PatchEntregaStatusCommand.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Entregas.Commands.PatchEntregaStatus;
public record PatchEntregaStatusCommand(Guid PedidoId, StatusEntrega NovoStatus) : IRequest<EntregaDto>;
```

```csharp
// AgentWorking.Application/Features/Entregas/Commands/PatchEntregaStatus/PatchEntregaStatusHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using AgentWorking.Domain.Enums;
using MediatR;
namespace AgentWorking.Application.Features.Entregas.Commands.PatchEntregaStatus;

public class PatchEntregaStatusHandler(
    IEntregaRepository repo,
    IPedidoRepository pedidoRepo,
    INotificacaoRepository notifRepo,
    IUnitOfWork uow) : IRequestHandler<PatchEntregaStatusCommand, EntregaDto>
{
    public async Task<EntregaDto> Handle(PatchEntregaStatusCommand cmd, CancellationToken ct)
    {
        var entrega = await repo.GetByPedidoAsync(cmd.PedidoId, ct)
            ?? throw new KeyNotFoundException($"Entrega for pedido {cmd.PedidoId} not found");

        var agora = DateTime.UtcNow;
        entrega.Status = cmd.NovoStatus;
        switch (cmd.NovoStatus)
        {
            case StatusEntrega.Saiu: entrega.TimestampSaiu = agora; break;
            case StatusEntrega.EmTransporte: entrega.TimestampTransporte = agora; break;
            case StatusEntrega.Entregue:
                entrega.TimestampEntregue = agora;
                var pedido = await pedidoRepo.GetByIdAsync(cmd.PedidoId, ct);
                if (pedido != null)
                {
                    pedido.Status = StatusPedido.Entregue;
                    pedidoRepo.Update(pedido);
                    await notifRepo.AddAsync(new Domain.Entities.Notificacao
                    {
                        Id = Guid.NewGuid(), UsuarioId = pedido.ClienteId,
                        Tipo = TipoNotificacao.Entrega, Titulo = "Pedido entregue",
                        Mensagem = "Seu pedido foi entregue com sucesso",
                        Lida = false, Timestamp = agora
                    }, ct);
                }
                break;
        }
        repo.Update(entrega);
        await uow.SaveChangesAsync(ct);

        return new EntregaDto(entrega.Id, entrega.PedidoId, entrega.Status.ToString(),
            entrega.TimestampSaiu, entrega.TimestampTransporte, entrega.TimestampEntregue);
    }
}
```

- [ ] **Step 3: Notificações**

```csharp
// AgentWorking.Application/Features/Notificacoes/Queries/GetNotificacoes/GetNotificacoesQuery.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Notificacoes.Queries.GetNotificacoes;
public record GetNotificacoesQuery(string UsuarioId) : IRequest<List<NotificacaoDto>>;
```

```csharp
// AgentWorking.Application/Features/Notificacoes/Queries/GetNotificacoes/GetNotificacoesHandler.cs
using AgentWorking.Application.DTOs;
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Notificacoes.Queries.GetNotificacoes;

public class GetNotificacoesHandler(INotificacaoRepository repo)
    : IRequestHandler<GetNotificacoesQuery, List<NotificacaoDto>>
{
    public async Task<List<NotificacaoDto>> Handle(GetNotificacoesQuery request, CancellationToken ct)
    {
        var notifs = await repo.GetByUsuarioAsync(request.UsuarioId, ct);
        return notifs.Select(n => new NotificacaoDto(
            n.Id, n.UsuarioId, n.Tipo.ToString(), n.Titulo, n.Mensagem, n.Lida, n.Timestamp)).ToList();
    }
}
```

```csharp
// AgentWorking.Application/Features/Notificacoes/Commands/PatchNotificacaoLida/PatchNotificacaoLidaCommand.cs
using MediatR;
namespace AgentWorking.Application.Features.Notificacoes.Commands.PatchNotificacaoLida;
public record PatchNotificacaoLidaCommand(Guid Id) : IRequest;
```

```csharp
// AgentWorking.Application/Features/Notificacoes/Commands/PatchNotificacaoLida/PatchNotificacaoLidaHandler.cs
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Notificacoes.Commands.PatchNotificacaoLida;

public class PatchNotificacaoLidaHandler(INotificacaoRepository repo, IUnitOfWork uow)
    : IRequestHandler<PatchNotificacaoLidaCommand>
{
    public async Task Handle(PatchNotificacaoLidaCommand cmd, CancellationToken ct)
    {
        var notif = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Notificacao {cmd.Id} not found");
        notif.Lida = true;
        repo.Update(notif);
        await uow.SaveChangesAsync(ct);
    }
}
```

```csharp
// AgentWorking.Application/Features/Notificacoes/Commands/PatchTodasLidas/PatchTodasLidasCommand.cs
using MediatR;
namespace AgentWorking.Application.Features.Notificacoes.Commands.PatchTodasLidas;
public record PatchTodasLidasCommand(string UsuarioId) : IRequest;
```

```csharp
// AgentWorking.Application/Features/Notificacoes/Commands/PatchTodasLidas/PatchTodasLidasHandler.cs
using AgentWorking.Application.Interfaces;
using MediatR;
namespace AgentWorking.Application.Features.Notificacoes.Commands.PatchTodasLidas;

public class PatchTodasLidasHandler(INotificacaoRepository repo)
    : IRequestHandler<PatchTodasLidasCommand>
{
    public async Task Handle(PatchTodasLidasCommand cmd, CancellationToken ct)
        => await repo.MarkAllAsReadAsync(cmd.UsuarioId, ct);
}
```

- [ ] **Step 4: Pagamentos (mocks)**

```csharp
// AgentWorking.Application/Features/Pagamentos/Commands/ProcessPix/ProcessPixCommand.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessPix;
public record ProcessPixCommand(Guid PedidoId, decimal Valor) : IRequest<PagamentoPixDto>;
```

```csharp
// AgentWorking.Application/Features/Pagamentos/Commands/ProcessPix/ProcessPixHandler.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessPix;

public class ProcessPixHandler : IRequestHandler<ProcessPixCommand, PagamentoPixDto>
{
    public Task<PagamentoPixDto> Handle(ProcessPixCommand request, CancellationToken ct)
    {
        // Mock: returns a static base64 1x1 pixel PNG as QR code
        const string mockQr = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
        return Task.FromResult(new PagamentoPixDto(
            QrCode: mockQr,
            ChavePix: "pagamentos@portalagro.com.br",
            Valor: request.Valor));
    }
}
```

```csharp
// AgentWorking.Application/Features/Pagamentos/Commands/ProcessCartao/ProcessCartaoCommand.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessCartao;
public record ProcessCartaoCommand(
    Guid PedidoId, string NumeroCartao, string NomeTitular,
    string Validade, string Cvv, decimal Valor) : IRequest<PagamentoCartaoDto>;
```

```csharp
// AgentWorking.Application/Features/Pagamentos/Commands/ProcessCartao/ProcessCartaoHandler.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessCartao;

public class ProcessCartaoHandler : IRequestHandler<ProcessCartaoCommand, PagamentoCartaoDto>
{
    public Task<PagamentoCartaoDto> Handle(ProcessCartaoCommand request, CancellationToken ct)
    {
        var auth = $"AUTH-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        return Task.FromResult(new PagamentoCartaoDto(Aprovado: true, CodigoAutorizacao: auth));
    }
}
```

```csharp
// AgentWorking.Application/Features/Pagamentos/Commands/ProcessCartao/ProcessCartaoValidator.cs
using FluentValidation;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessCartao;
public class ProcessCartaoValidator : AbstractValidator<ProcessCartaoCommand>
{
    public ProcessCartaoValidator()
    {
        RuleFor(x => x.NumeroCartao).NotEmpty().Length(16)
            .Matches(@"^\d{16}$").WithMessage("Número do cartão deve ter 16 dígitos");
        RuleFor(x => x.NomeTitular).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Validade).NotEmpty().Matches(@"^\d{2}/\d{2}$")
            .WithMessage("Validade deve ser MM/AA");
        RuleFor(x => x.Cvv).NotEmpty().Length(3, 4).Matches(@"^\d{3,4}$");
        RuleFor(x => x.Valor).GreaterThan(0);
    }
}
```

```csharp
// AgentWorking.Application/Features/Pagamentos/Commands/ProcessBoleto/ProcessBoletoCommand.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessBoleto;
public record ProcessBoletoCommand(Guid PedidoId, decimal Valor) : IRequest<PagamentoBoletoDto>;
```

```csharp
// AgentWorking.Application/Features/Pagamentos/Commands/ProcessBoleto/ProcessBoletoHandler.cs
using AgentWorking.Application.DTOs;
using MediatR;
namespace AgentWorking.Application.Features.Pagamentos.Commands.ProcessBoleto;

public class ProcessBoletoHandler : IRequestHandler<ProcessBoletoCommand, PagamentoBoletoDto>
{
    public Task<PagamentoBoletoDto> Handle(ProcessBoletoCommand request, CancellationToken ct)
    {
        var codigo = $"34191.09008 61234.678901 23456.789012 3 {(long)(request.Valor * 100):D17}";
        return Task.FromResult(new PagamentoBoletoDto(
            CodigoBarras: codigo.Replace(" ", ""),
            LinhaDigitavel: codigo,
            PdfUrl: $"/api/pagamentos/boleto/{request.PedidoId}/pdf"));
    }
}
```

- [ ] **Step 5: Verify build**

```bash
cd /root/api-agent-working && dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 6: Commit**

```bash
git -C /root/api-agent-working add -A
git -C /root/api-agent-working commit -m "feat(app): add Vendas, Entregas, Notificacoes, and Pagamentos features"
```

---

## Task 13: PedidoExpirationJob (IHostedService)

**Files:**
- Create: `AgentWorking.Infrastructure/BackgroundJobs/PedidoExpirationJob.cs`
- Modify: `AgentWorking.Infrastructure/DependencyInjection.cs`

- [ ] **Step 1: Create the background job**

```csharp
// AgentWorking.Infrastructure/BackgroundJobs/PedidoExpirationJob.cs
using AgentWorking.Application.Features.Pedidos.Commands.ExpirePedidos;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AgentWorking.Infrastructure.BackgroundJobs;

public class PedidoExpirationJob(
    IServiceScopeFactory scopeFactory,
    ILogger<PedidoExpirationJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new ExpirePedidosCommand(), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in PedidoExpirationJob");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

- [ ] **Step 2: Register job in Infrastructure DI**

Add to `AgentWorking.Infrastructure/DependencyInjection.cs` before `return services;`:

```csharp
services.AddHostedService<AgentWorking.Infrastructure.BackgroundJobs.PedidoExpirationJob>();
```

Also add the using at the top of `DependencyInjection.cs`:

```csharp
using AgentWorking.Infrastructure.BackgroundJobs;
```

- [ ] **Step 3: Verify build**

```bash
cd /root/api-agent-working && dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```bash
git -C /root/api-agent-working add -A
git -C /root/api-agent-working commit -m "feat(infra): add PedidoExpirationJob background service"
```

---

## Task 14: Controllers

**Files:**
- Create: `AgentWorking.API/Controllers/CentrosController.cs`
- Create: `AgentWorking.API/Controllers/ProdutosController.cs`
- Create: `AgentWorking.API/Controllers/ComprasController.cs`
- Create: `AgentWorking.API/Controllers/LotesController.cs`
- Create: `AgentWorking.API/Controllers/PedidosController.cs`
- Create: `AgentWorking.API/Controllers/VendasController.cs`
- Create: `AgentWorking.API/Controllers/EntregasController.cs`
- Create: `AgentWorking.API/Controllers/PagamentosController.cs`
- Create: `AgentWorking.API/Controllers/NotificacoesController.cs`
- Delete: `AgentWorking.API/Controllers/WeatherForecastController.cs`
- Delete: `AgentWorking.API/WeatherForecast.cs`

- [ ] **Step 1: Delete scaffold files**

```bash
rm /root/api-agent-working/AgentWorking.API/Controllers/WeatherForecastController.cs
rm /root/api-agent-working/AgentWorking.API/WeatherForecast.cs
```

- [ ] **Step 2: Create CentrosController**

```csharp
// AgentWorking.API/Controllers/CentrosController.cs
using AgentWorking.Application.Features.Centros.Queries.GetCentros;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CentrosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await mediator.Send(new GetCentrosQuery(), ct));
}
```

- [ ] **Step 3: Create ProdutosController**

```csharp
// AgentWorking.API/Controllers/ProdutosController.cs
using AgentWorking.Application.Features.Produtos.Commands.CreateProduto;
using AgentWorking.Application.Features.Produtos.Commands.PatchProdutoStatus;
using AgentWorking.Application.Features.Produtos.Commands.UpdateProduto;
using AgentWorking.Application.Features.Produtos.Queries.GetProdutos;
using AgentWorking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? centroId, [FromQuery] string? produtorId, CancellationToken ct)
        => Ok(await mediator.Send(new GetProdutosQuery(centroId, produtorId), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProdutoCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProdutoRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new UpdateProdutoCommand(
            id, req.Nome, req.Categoria, req.Quantidade, req.Unidade,
            req.Preco, req.Safra, req.Cidade, req.Foto), ct));

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> PatchStatus(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new PatchProdutoStatusCommand(id), ct));
}

public record UpdateProdutoRequest(
    string Nome, Categoria Categoria, decimal Quantidade, UnidadeMedida Unidade,
    decimal Preco, string Safra, string Cidade, string? Foto);
```

- [ ] **Step 4: Create ComprasController**

```csharp
// AgentWorking.API/Controllers/ComprasController.cs
using AgentWorking.Application.Features.Compras.Commands.CreateCompra;
using AgentWorking.Application.Features.Compras.Queries.GetCompras;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComprasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string compradorId, CancellationToken ct)
        => Ok(await mediator.Send(new GetComprasQuery(compradorId), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompraCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetAll), new { compradorId = cmd.CompradorId }, result);
    }
}
```

- [ ] **Step 5: Create LotesController**

```csharp
// AgentWorking.API/Controllers/LotesController.cs
using AgentWorking.Application.Features.Lotes.Commands.UpdateLote;
using AgentWorking.Application.Features.Lotes.Queries.GetCatalogo;
using AgentWorking.Application.Features.Lotes.Queries.GetLotes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LotesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByComprador([FromQuery] string compradorId, CancellationToken ct)
        => Ok(await mediator.Send(new GetLotesQuery(compradorId), ct));

    [HttpGet("catalogo")]
    public async Task<IActionResult> GetCatalogo([FromQuery] string? categoria, CancellationToken ct)
        => Ok(await mediator.Send(new GetCatalogoQuery(categoria), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLoteRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new UpdateLoteCommand(id, req.Quantidade, req.PrecoVenda, req.Validade), ct));
}

public record UpdateLoteRequest(decimal Quantidade, decimal PrecoVenda, DateTime Validade);
```

- [ ] **Step 6: Create PedidosController**

```csharp
// AgentWorking.API/Controllers/PedidosController.cs
using AgentWorking.Application.Features.Pedidos.Commands.CreatePedido;
using AgentWorking.Application.Features.Pedidos.Commands.CreatePedidoPersonalizado;
using AgentWorking.Application.Features.Pedidos.Commands.PatchPedidoStatus;
using AgentWorking.Application.Features.Pedidos.Queries.GetPedidos;
using AgentWorking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? compradorId, [FromQuery] string? clienteId, CancellationToken ct)
        => Ok(await mediator.Send(new GetPedidosQuery(compradorId, clienteId), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePedidoCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    [HttpPost("personalizado")]
    public async Task<IActionResult> CreatePersonalizado(
        [FromBody] CreatePedidoPersonalizadoCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> PatchStatus(
        Guid id, [FromBody] PatchStatusRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new PatchPedidoStatusCommand(id, req.NovoStatus, req.NovaDataEntrega), ct));
}

public record PatchStatusRequest(StatusPedido NovoStatus, DateTime? NovaDataEntrega);
```

- [ ] **Step 7: Create remaining controllers**

```csharp
// AgentWorking.API/Controllers/VendasController.cs
using AgentWorking.Application.Features.Vendas.Queries.GetVendas;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string produtorId, CancellationToken ct)
        => Ok(await mediator.Send(new GetVendasQuery(produtorId), ct));
}
```

```csharp
// AgentWorking.API/Controllers/EntregasController.cs
using AgentWorking.Application.Features.Entregas.Commands.PatchEntregaStatus;
using AgentWorking.Application.Features.Entregas.Queries.GetEntrega;
using AgentWorking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EntregasController(IMediator mediator) : ControllerBase
{
    [HttpGet("{pedidoId:guid}")]
    public async Task<IActionResult> GetByPedido(Guid pedidoId, CancellationToken ct)
        => Ok(await mediator.Send(new GetEntregaQuery(pedidoId), ct));

    [HttpPatch("{pedidoId:guid}/status")]
    public async Task<IActionResult> PatchStatus(
        Guid pedidoId, [FromBody] PatchEntregaRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new PatchEntregaStatusCommand(pedidoId, req.NovoStatus), ct));
}

public record PatchEntregaRequest(StatusEntrega NovoStatus);
```

```csharp
// AgentWorking.API/Controllers/PagamentosController.cs
using AgentWorking.Application.Features.Pagamentos.Commands.ProcessBoleto;
using AgentWorking.Application.Features.Pagamentos.Commands.ProcessCartao;
using AgentWorking.Application.Features.Pagamentos.Commands.ProcessPix;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagamentosController(IMediator mediator) : ControllerBase
{
    [HttpPost("pix")]
    public async Task<IActionResult> Pix([FromBody] ProcessPixCommand cmd, CancellationToken ct)
        => Ok(await mediator.Send(cmd, ct));

    [HttpPost("cartao")]
    public async Task<IActionResult> Cartao([FromBody] ProcessCartaoCommand cmd, CancellationToken ct)
        => Ok(await mediator.Send(cmd, ct));

    [HttpPost("boleto")]
    public async Task<IActionResult> Boleto([FromBody] ProcessBoletoCommand cmd, CancellationToken ct)
        => Ok(await mediator.Send(cmd, ct));
}
```

```csharp
// AgentWorking.API/Controllers/NotificacoesController.cs
using AgentWorking.Application.Features.Notificacoes.Commands.PatchNotificacaoLida;
using AgentWorking.Application.Features.Notificacoes.Commands.PatchTodasLidas;
using AgentWorking.Application.Features.Notificacoes.Queries.GetNotificacoes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacoesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string usuarioId, CancellationToken ct)
        => Ok(await mediator.Send(new GetNotificacoesQuery(usuarioId), ct));

    [HttpPatch("{id:guid}/lida")]
    public async Task<IActionResult> MarcarLida(Guid id, CancellationToken ct)
    {
        await mediator.Send(new PatchNotificacaoLidaCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("lidas")]
    public async Task<IActionResult> MarcarTodasLidas([FromQuery] string usuarioId, CancellationToken ct)
    {
        await mediator.Send(new PatchTodasLidasCommand(usuarioId), ct);
        return NoContent();
    }
}
```

- [ ] **Step 8: Verify build**

```bash
cd /root/api-agent-working && dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 9: Commit**

```bash
git -C /root/api-agent-working add -A
git -C /root/api-agent-working commit -m "feat(api): add all controllers, remove WeatherForecast scaffold"
```

---

## Task 15: Program.cs, middleware, CORS, Swagger, Problem Details

**Files:**
- Modify: `AgentWorking.API/Program.cs`
- Create: `AgentWorking.API/Middleware/ExceptionHandlerMiddleware.cs`

- [ ] **Step 1: Create exception handler middleware**

```csharp
// AgentWorking.API/Middleware/ExceptionHandlerMiddleware.cs
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AgentWorking.API.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/problem+json";
            var problem = new ValidationProblemDetails
            {
                Type = "validation_error",
                Title = "One or more validation errors occurred.",
                Status = 400
            };
            foreach (var failure in ex.Errors)
                problem.Errors.TryAdd(failure.PropertyName, [failure.ErrorMessage]);

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
        catch (KeyNotFoundException ex)
        {
            context.Response.StatusCode = 404;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {
                Type = "not_found", Title = ex.Message, Status = 404
            }));
        }
        catch (InvalidOperationException ex)
        {
            context.Response.StatusCode = 409;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {
                Type = "conflict", Title = ex.Message, Status = 409
            }));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {
                Type = "internal_error",
                Title = "An unexpected error occurred.",
                Status = 500
            }));
        }
    }
}
```

- [ ] **Step 2: Update Program.cs (final version)**

```csharp
// AgentWorking.API/Program.cs
using AgentWorking.API.Middleware;
using AgentWorking.Application;
using AgentWorking.Infrastructure;
using AgentWorking.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Portal Agro API", Version = "v1" });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedData.SeedAsync(db);
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portal Agro API v1"));
}

app.UseCors("FrontendPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();
```

- [ ] **Step 3: Final build check**

```bash
cd /root/api-agent-working && dotnet build
```

Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 4: Run all tests**

```bash
cd /root/api-agent-working && dotnet test AgentWorking.Tests -v
```

Expected: `Passed! - Failed: 0, Passed: 8`

- [ ] **Step 5: Start API and verify Swagger**

```bash
cd /root/api-agent-working && dotnet run --project AgentWorking.API &
sleep 4
curl -s http://localhost:5000/api/centros
kill %1
```

Expected: JSON array with seeded Centros

- [ ] **Step 6: Final commit**

```bash
git -C /root/api-agent-working add -A
git -C /root/api-agent-working commit -m "feat(api): final Program.cs with CORS, Swagger, exception middleware"
```

---

## Self-Review Checklist

- [x] `CentroDistribuicao` entity + seeded data ✓
- [x] `Produto.CentroDistribuicaoId` + `GET /api/produtos?centroId=` ✓
- [x] `Compra` flow: validates stock → decrements `Produto.Quantidade` → creates `LoteEstoque` → notifies Produtor ✓
- [x] Catalog filter: `Validade > NOW() AND Quantidade > 0` in `LoteRepository.GetCatalogoAsync` ✓
- [x] Transactional checkout: validates all lotes first, then mutates → `SaveChangesAsync` once ✓
- [x] `PedidoPersonalizado`: `PrazoAceite = DataCriacao + 24h`, broadcasts to compradores ✓
- [x] `ExpirePedidosHandler`: sets `Expirado`, notifies cliente, called by `PedidoExpirationJob` every 5 min ✓
- [x] `Venda` created per checkout item linking `PedidoId` → `CompraId` → `ProdutorId` ✓
- [x] `GET /api/vendas?produtorId=` for Produtor sales history ✓
- [x] Entrega: 3-step status update with timestamps, marks Pedido `Entregue` on final step ✓
- [x] Pagamentos: 3 mock endpoints (Pix QR base64, Cartão Luhn-validated mock, Boleto linha digitável) ✓
- [x] CORS: `localhost:5173` + `localhost:3000` ✓
- [x] `ValidationBehavior` pipeline returns 400 Problem Details ✓
- [x] `KeyNotFoundException` → 404, `InvalidOperationException` → 409 ✓
