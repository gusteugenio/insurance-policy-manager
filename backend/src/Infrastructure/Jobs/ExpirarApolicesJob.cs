using InsurancePolicyManager.Domain.Enums;
using InsurancePolicyManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InsurancePolicyManager.Infrastructure.Jobs;

public class ExpirarApolicesJob : BackgroundService
{
  private readonly IServiceProvider _serviceProvider;
  private readonly ILogger<ExpirarApolicesJob> _logger;
  private static readonly TimeSpan Intervalo = TimeSpan.FromHours(24);

  public ExpirarApolicesJob(IServiceProvider serviceProvider, ILogger<ExpirarApolicesJob> logger)
  {
    _serviceProvider = serviceProvider;
    _logger = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      await ExpirarApolicesVencidasAsync(stoppingToken);
      await Task.Delay(Intervalo, stoppingToken);
    }
  }

  private async Task ExpirarApolicesVencidasAsync(CancellationToken cancellationToken)
  {
    using var scope = _serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var apolicesVencidas = await context.Apolices
      .Where(a => a.Status == StatusApolice.Ativa && a.DataFim.Date < DateTime.UtcNow.Date)
      .ToListAsync(cancellationToken);

    if (apolicesVencidas.Count == 0)
      return;

    foreach (var apolice in apolicesVencidas)
      apolice.Expirar();

    await context.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("{Quantidade} apólice(s) expirada(s) automaticamente pelo job de expiração.", apolicesVencidas.Count);
  }
}
