using InsurancePolicyManager.Domain.Entities;
using InsurancePolicyManager.Domain.Enums;

namespace InsurancePolicyManager.Domain.Interfaces;

public interface IApoliceRepository
{
  Task<Apolice?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
  Task<(IEnumerable<Apolice> Itens, int Total)> ListarAsync(
    int pagina, int tamanhoPagina, StatusApolice? status, string? ordenarPor,
    CancellationToken cancellationToken = default);
  Task<IEnumerable<Apolice>> ListarVencendoEmAsync(int dias, CancellationToken cancellationToken = default);
  Task AdicionarAsync(Apolice apolice, CancellationToken cancellationToken = default);
  Task AtualizarAsync(Apolice apolice, CancellationToken cancellationToken = default);
  Task RemoverAsync(Apolice apolice, CancellationToken cancellationToken = default);
}
