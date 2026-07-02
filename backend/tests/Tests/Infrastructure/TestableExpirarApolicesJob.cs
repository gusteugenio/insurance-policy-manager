using InsurancePolicyManager.Domain.Enums;
using InsurancePolicyManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InsurancePolicyManager.Tests.Infrastructure;

// Expõe a lógica de expiração para teste direto, sem depender do loop infinito do BackgroundService.
public class TestableExpirarApolicesJob
{
  private readonly IServiceProvider _serviceProvider;
  private readonly ILogger _logger;

  public TestableExpirarApolicesJob(IServiceProvider serviceProvider, ILogger logger)
  {
    _serviceProvider = serviceProvider;
    _logger = logger;
  }

  public async Task ExecutarUmaVezAsync(CancellationToken cancellationToken)
  {
    using var scope = _serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var apolicesVencidas = await context.Apolices
      .Where(a => a.Status == StatusApolice.Ativa && a.DataFim.Date < DateTime.UtcNow.Date)
      .ToListAsync(cancellationToken);

    foreach (var apolice in apolicesVencidas)
      apolice.Expirar();

    if (apolicesVencidas.Count > 0)
      await context.SaveChangesAsync(cancellationToken);
  }
}
