using InsurancePolicyManager.Application.DTOs;

namespace InsurancePolicyManager.Application.Interfaces;

public interface IClienteService
{
  Task<ClienteDto?> ObterPorDocumentoAsync(string documento, CancellationToken cancellationToken = default);
}
