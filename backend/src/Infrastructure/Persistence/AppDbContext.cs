using Microsoft.EntityFrameworkCore;

namespace InsurancePolicyManager.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
