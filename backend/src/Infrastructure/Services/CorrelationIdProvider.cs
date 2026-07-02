using InsurancePolicyManager.Application.Common;
using Microsoft.AspNetCore.Http;

namespace InsurancePolicyManager.Infrastructure.Services;

public class CorrelationIdProvider : ICorrelationIdProvider
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public CorrelationIdProvider(IHttpContextAccessor httpContextAccessor)
    => _httpContextAccessor = httpContextAccessor;

  public string GetCorrelationId()
    => _httpContextAccessor.HttpContext?.Items["CorrelationId"] as string ?? "N/A";
}
