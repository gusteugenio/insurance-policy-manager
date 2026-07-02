using InsurancePolicyManager.Domain.Entities;
using InsurancePolicyManager.Domain.Enums;
using InsurancePolicyManager.Domain.Interfaces;
using InsurancePolicyManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InsurancePolicyManager.Infrastructure.Repositories;

public class ApoliceRepository : IApoliceRepository
{
    private readonly AppDbContext _context;

    public ApoliceRepository(AppDbContext context) => _context = context;

    public async Task<Apolice?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Apolices
            .Include(a => a.Cliente)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<(IEnumerable<Apolice> Itens, int Total)> ListarAsync(
        int pagina, int tamanhoPagina, StatusApolice? status, string? ordenarPor,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Apolices.Include(a => a.Cliente).AsQueryable();

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        query = ordenarPor?.ToLower() switch
        {
            "datafim" => query.OrderBy(a => a.DataFim),
            "valorpremio" => query.OrderBy(a => a.ValorPremio),
            _ => query.OrderByDescending(a => a.DataInicio)
        };

        var total = await query.CountAsync(cancellationToken);

        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return (itens, total);
    }

    // Consulta de apólices vencendo em N dias
    public async Task<IEnumerable<Apolice>> ListarVencendoEmAsync(int dias, CancellationToken cancellationToken = default)
    {
        var limite = DateTime.UtcNow.Date.AddDays(dias);

        return await _context.Apolices
            .Include(a => a.Cliente)
            .Where(a => a.Status == StatusApolice.Ativa && a.DataFim.Date <= limite)
            .OrderBy(a => a.DataFim)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Apolice apolice, CancellationToken cancellationToken = default)
    {
        await _context.Apolices.AddAsync(apolice, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Apolice apolice, CancellationToken cancellationToken = default)
    {
        _context.Apolices.Update(apolice);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Apolice apolice, CancellationToken cancellationToken = default)
    {
        _context.Apolices.Remove(apolice);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
