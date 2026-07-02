using InsurancePolicyManager.Application.Common;
using InsurancePolicyManager.Application.DTOs;

namespace InsurancePolicyManager.Application.Interfaces;

public interface IApoliceService
{
  Task<ApoliceDto> CriarAsync(CriarApoliceDto dto, CancellationToken cancellationToken = default);
  Task<PagedResult<ApoliceDto>> ListarAsync(int pagina, int tamanhoPagina, string? status, string? ordenarPor, CancellationToken cancellationToken = default);
  Task<ApoliceDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ApoliceDto> AtualizarAsync(Guid id, AtualizarApoliceDto dto, CancellationToken cancellationToken = default);
  Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IEnumerable<ApoliceDto>> ListarVencendoEmAsync(int dias, CancellationToken cancellationToken = default);
}
