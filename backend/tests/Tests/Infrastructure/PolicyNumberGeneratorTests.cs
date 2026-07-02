using InsurancePolicyManager.Domain.Entities;
using InsurancePolicyManager.Infrastructure.Persistence;
using InsurancePolicyManager.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InsurancePolicyManager.Tests.Infrastructure;

public class PolicyNumberGeneratorTests
{
  private static AppDbContext CriarContextoEmMemoria()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString())
      .Options;

    return new AppDbContext(options);
  }

  [Fact]
  public async Task GerarNumeroAsync_QuandoNaoHaApolicesNoAno_DeveGerarPrimeiroNumero()
  {
    await using var context = CriarContextoEmMemoria();
    var generator = new PolicyNumberGenerator(context);

    var numero = await generator.GerarNumeroAsync();

    Assert.Equal($"SEG-{DateTime.UtcNow.Year}-0001", numero);
  }

  [Fact]
  public async Task GerarNumeroAsync_QuandoJaExistemApolicesNoAno_DeveIncrementarSequencial()
  {
    await using var context = CriarContextoEmMemoria();
    var cliente = new Cliente("12345678900", "João da Silva");
    context.Clientes.Add(cliente);

    var ano = DateTime.UtcNow.Year;
    context.Apolices.Add(new Apolice($"SEG-{ano}-0001", cliente.Id, "ABC1D23", 150m, DateTime.UtcNow, DateTime.UtcNow.AddMonths(12)));
    context.Apolices.Add(new Apolice($"SEG-{ano}-0002", cliente.Id, "XYZ9K88", 200m, DateTime.UtcNow, DateTime.UtcNow.AddMonths(12)));
    await context.SaveChangesAsync();

    var generator = new PolicyNumberGenerator(context);

    var numero = await generator.GerarNumeroAsync();

    Assert.Equal($"SEG-{ano}-0003", numero);
  }
}
