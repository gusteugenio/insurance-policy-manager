using InsurancePolicyManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsurancePolicyManager.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<Cliente> Clientes => Set<Cliente>();
  public DbSet<Apolice> Apolices => Set<Apolice>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    base.OnModelCreating(modelBuilder);
  }
}
