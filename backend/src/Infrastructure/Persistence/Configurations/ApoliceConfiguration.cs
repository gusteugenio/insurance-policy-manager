using InsurancePolicyManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsurancePolicyManager.Infrastructure.Persistence.Configurations;

public class ApoliceConfiguration : IEntityTypeConfiguration<Apolice>
{
  public void Configure(EntityTypeBuilder<Apolice> builder)
  {
    builder.ToTable("Apolices");
    builder.HasKey(a => a.Id);

    builder.Property(a => a.Numero).IsRequired().HasMaxLength(20);
    builder.HasIndex(a => a.Numero).IsUnique();

    builder.Property(a => a.Placa).IsRequired().HasMaxLength(8);
    builder.Property(a => a.ValorPremio).HasColumnType("decimal(18,2)");
    builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);

    // Índices para otimizar consultas frequentes
    builder.HasIndex(a => a.Status);
    builder.HasIndex(a => a.DataFim);
    builder.HasIndex(a => a.ClienteId);
  }
}
