using InsurancePolicyManager.Domain.Entities;

namespace InsurancePolicyManager.Domain.Interfaces;

public interface IClienteRepository
{
  Task<Cliente?> ObterPorDocumentoAsync(string documento, CancellationToken cancellationToken = default);
  Task<(IEnumerable<Cliente> Itens, int Total)> ListarAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default);
  Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default);
}
