using InsurancePolicyManager.Domain.Entities;
using InsurancePolicyManager.Domain.Interfaces;
using InsurancePolicyManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InsurancePolicyManager.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
  private readonly AppDbContext _context;

  public ClienteRepository(AppDbContext context) => _context = context;

  public async Task<Cliente?> ObterPorDocumentoAsync(string documento, CancellationToken cancellationToken = default)
    => await _context.Clientes.FirstOrDefaultAsync(c => c.Documento == documento, cancellationToken);

  public async Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
  {
    await _context.Clientes.AddAsync(cliente, cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
