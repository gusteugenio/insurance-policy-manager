using InsurancePolicyManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsurancePolicyManager.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
  public void Configure(EntityTypeBuilder<Cliente> builder)
  {
    builder.ToTable("Clientes");
    builder.HasKey(c => c.Id);

    builder.Property(c => c.Documento).IsRequired().HasMaxLength(18);
    builder.Property(c => c.Nome).IsRequired().HasMaxLength(150);

    builder.HasIndex(c => c.Documento).IsUnique();

    builder.HasMany(c => c.Apolices)
      .WithOne(a => a.Cliente)
      .HasForeignKey(a => a.ClienteId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
