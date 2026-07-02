using InsurancePolicyManager.Domain.Entities;
using InsurancePolicyManager.Domain.Enums;
using InsurancePolicyManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace InsurancePolicyManager.Tests.Infrastructure;

public class ExpirarApolicesJobTests
{
  private static ServiceProvider CriarProviderComContexto(out AppDbContext context)
  {
    var databaseName = Guid.NewGuid().ToString();
    var services = new ServiceCollection();
    services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(databaseName));
    var provider = services.BuildServiceProvider();

    context = provider.GetRequiredService<AppDbContext>();
    return provider;
  }

  [Fact]
  public async Task ExpirarApolicesVencidasAsync_ComApoliceAtivaVencida_DeveMarcarComoExpirada()
  {
    var provider = CriarProviderComContexto(out var context);

    var cliente = new Cliente("12345678900", "João da Silva");
    var apolice = new Apolice("SEG-2026-0001", cliente.Id, "ABC1D23", 150m, DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddDays(-1));
    context.Clientes.Add(cliente);
    context.Apolices.Add(apolice);
    await context.SaveChangesAsync();

    var job = new TestableExpirarApolicesJob(provider, NullLogger<InsurancePolicyManager.Infrastructure.Jobs.ExpirarApolicesJob>.Instance);
    await job.ExecutarUmaVezAsync(CancellationToken.None);

    var apoliceAtualizada = await context.Apolices
      .AsNoTracking()
      .FirstAsync(a => a.Id == apolice.Id);
    Assert.Equal(StatusApolice.Expirada, apoliceAtualizada.Status);
  }

  [Fact]
  public async Task ExpirarApolicesVencidasAsync_ComApoliceAtivaDentroDoPrazo_NaoDeveAlterarStatus()
  {
    var provider = CriarProviderComContexto(out var context);

    var cliente = new Cliente("12345678900", "João da Silva");
    var apolice = new Apolice("SEG-2026-0001", cliente.Id, "ABC1D23", 150m, DateTime.UtcNow, DateTime.UtcNow.AddMonths(6));
    context.Clientes.Add(cliente);
    context.Apolices.Add(apolice);
    await context.SaveChangesAsync();

    var job = new TestableExpirarApolicesJob(provider, NullLogger<InsurancePolicyManager.Infrastructure.Jobs.ExpirarApolicesJob>.Instance);
    await job.ExecutarUmaVezAsync(CancellationToken.None);

    var apoliceAtualizada = await context.Apolices.FirstAsync(a => a.Id == apolice.Id);
    Assert.Equal(StatusApolice.Ativa, apoliceAtualizada.Status);
  }
}
