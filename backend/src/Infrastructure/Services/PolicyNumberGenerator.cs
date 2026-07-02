using InsurancePolicyManager.Domain.Interfaces;
using InsurancePolicyManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InsurancePolicyManager.Infrastructure.Services;

public class PolicyNumberGenerator : IPolicyNumberGenerator
{
  private readonly AppDbContext _context;

  public PolicyNumberGenerator(AppDbContext context)
  {
    _context = context;
  }

  public async Task<string> GerarNumeroAsync(CancellationToken cancellationToken = default)
  {
    var ano = DateTime.UtcNow.Year;
    var prefixo = $"SEG-{ano}-";

    var quantidadeNoAno = await _context.Apolices
      .Where(a => a.Numero.StartsWith(prefixo))
      .CountAsync(cancellationToken);

    var proximoNumero = quantidadeNoAno + 1;
    return $"{prefixo}{proximoNumero:D4}";
  }
}
