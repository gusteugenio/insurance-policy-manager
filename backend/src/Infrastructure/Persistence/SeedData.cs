using InsurancePolicyManager.Domain.Entities;

namespace InsurancePolicyManager.Infrastructure.Persistence;

public static class SeedData
{
  public static void Seed(AppDbContext context)
  {
    if (context.Clientes.Any())
      return;

    var cliente1 = new Cliente("123.456.789-00", "João da Silva");
    var cliente2 = new Cliente("987.654.321-00", "Maria Oliveira");
    context.Clientes.AddRange(cliente1, cliente2);
    context.SaveChanges();

    var apolice1 = new Apolice("SEG-2026-0001", cliente1.Id, "ABC1D23", 150.00m, DateTime.UtcNow.AddMonths(-2), DateTime.UtcNow.AddDays(15));
    var apolice2 = new Apolice("SEG-2026-0002", cliente2.Id, "XYZ9K88", 220.50m, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow.AddMonths(11));
    context.Apolices.AddRange(apolice1, apolice2);
    context.SaveChanges();
  }
}
