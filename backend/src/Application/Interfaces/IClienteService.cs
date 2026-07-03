using InsurancePolicyManager.Application.Common;
using InsurancePolicyManager.Application.DTOs;

namespace InsurancePolicyManager.Application.Interfaces;

public interface IClienteService
{
  Task<ClienteDto?> ObterPorDocumentoAsync(string documento, CancellationToken cancellationToken = default);
  Task<PagedResult<ClienteDto>> ListarAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default);
}
