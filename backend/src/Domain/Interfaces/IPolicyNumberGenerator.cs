namespace InsurancePolicyManager.Domain.Interfaces;

public interface IPolicyNumberGenerator
{
  Task<string> GerarNumeroAsync(CancellationToken cancellationToken = default);
}
