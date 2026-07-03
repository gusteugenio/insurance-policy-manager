using InsurancePolicyManager.Domain.Entities;
using InsurancePolicyManager.Infrastructure.Persistence;
using InsurancePolicyManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InsurancePolicyManager.Tests.Infrastructure;

public class ApoliceRepositoryTests
{
  private static AppDbContext CriarContextoEmMemoria()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString())
      .Options;

    return new AppDbContext(options);
  }

  [Fact]
  public async Task ListarAsync_ComClienteIdInformado_DeveRetornarApenasApolicesDaqueleCliente()
  {
    await using var context = CriarContextoEmMemoria();

    var cliente1 = new Cliente("12345678900", "João da Silva");
    var cliente2 = new Cliente("98765432100", "Maria Oliveira");
    context.Clientes.AddRange(cliente1, cliente2);

    context.Apolices.AddRange(
      new Apolice("SEG-2026-0001", cliente1.Id, "ABC1D23", 150m, DateTime.UtcNow, DateTime.UtcNow.AddMonths(12)),
      new Apolice("SEG-2026-0002", cliente1.Id, "XYZ9K88", 200m, DateTime.UtcNow, DateTime.UtcNow.AddMonths(12)),
      new Apolice("SEG-2026-0003", cliente2.Id, "JJJ0L11", 180m, DateTime.UtcNow, DateTime.UtcNow.AddMonths(12)));
    await context.SaveChangesAsync();

    var repository = new ApoliceRepository(context);

    var (itens, total) = await repository.ListarAsync(1, 10, null, cliente1.Id, null);

    Assert.Equal(2, total);
    Assert.All(itens, a => Assert.Equal(cliente1.Id, a.ClienteId));
  }

  [Fact]
  public async Task ListarAsync_OrdenadoPorDataInicio_DeveRetornarEmOrdemCrescente()
  {
    await using var context = CriarContextoEmMemoria();

    var cliente = new Cliente("12345678900", "João da Silva");
    context.Clientes.Add(cliente);

    context.Apolices.AddRange(
      new Apolice("SEG-2026-0001", cliente.Id, "ABC1D23", 150m, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddMonths(12)),
      new Apolice("SEG-2026-0002", cliente.Id, "XYZ9K88", 200m, DateTime.UtcNow.AddDays(-20), DateTime.UtcNow.AddMonths(12)));
    await context.SaveChangesAsync();

    var repository = new ApoliceRepository(context);

    var (itens, _) = await repository.ListarAsync(1, 10, null, null, "datainicio");
    var lista = itens.ToList();

    Assert.True(lista[0].DataInicio < lista[1].DataInicio);
  }
}
