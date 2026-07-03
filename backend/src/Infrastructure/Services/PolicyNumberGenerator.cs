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

    var numeros = await _context.Apolices
      .Where(a => a.Numero.StartsWith(prefixo))
      .Select(a => a.Numero)
      .ToListAsync(cancellationToken);

    var maiorSequencial = numeros
      .Select(n => int.TryParse(n.AsSpan(prefixo.Length), out var seq) ? seq : 0)
      .DefaultIfEmpty(0)
      .Max();

    var proximoNumero = maiorSequencial + 1;
    return $"{prefixo}{proximoNumero:D4}";
  }
}
