using InsurancePolicyManager.Domain.Entities;
using InsurancePolicyManager.Domain.Interfaces;
using InsurancePolicyManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InsurancePolicyManager.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
  private readonly AppDbContext _context;

  public ClienteRepository(AppDbContext context) => _context = context;

  public async Task<(IEnumerable<Cliente> Itens, int Total)> ListarAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default)
  {
    var query = _context.Clientes.OrderBy(c => c.Nome).AsQueryable();

    var total = await query.CountAsync(cancellationToken);

    var itens = await query
      .Skip((pagina - 1) * tamanhoPagina)
      .Take(tamanhoPagina)
      .ToListAsync(cancellationToken);

    return (itens, total);
  }

  public async Task<Cliente?> ObterPorDocumentoAsync(string documento, CancellationToken cancellationToken = default)
  {
    var documentoNormalizado = Cliente.NormalizarDocumento(documento);
    return await _context.Clientes.FirstOrDefaultAsync(c => c.Documento == documentoNormalizado, cancellationToken);
  }

  public async Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
  {
    await _context.Clientes.AddAsync(cliente, cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
