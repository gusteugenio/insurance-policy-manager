namespace InsurancePolicyManager.Application.Common;

public interface ICorrelationIdProvider
{
  string GetCorrelationId();
}
