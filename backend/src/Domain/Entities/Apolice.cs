using InsurancePolicyManager.Domain.Enums;
using InsurancePolicyManager.Domain.Exceptions;

namespace InsurancePolicyManager.Domain.Entities;

public class Apolice
{
  public Guid Id { get; private set; }
  public string Numero { get; private set; } = string.Empty;
  public Guid ClienteId { get; private set; }
  public Cliente? Cliente { get; private set; }
  public string Placa { get; private set; } = string.Empty;
  public decimal ValorPremio { get; private set; }
  public DateTime DataInicio { get; private set; }
  public DateTime DataFim { get; private set; }
  public StatusApolice Status { get; private set; }

  protected Apolice() { } // uso exclusivo do EF Core

  public Apolice(string numero, Guid clienteId, string placa, decimal valorPremio, DateTime dataInicio, DateTime dataFim)
  {
    if (string.IsNullOrWhiteSpace(numero))
      throw new DomainException("Número da apólice é obrigatório.");

    if (string.IsNullOrWhiteSpace(placa))
      throw new DomainException("Placa do veículo é obrigatória.");

    if (valorPremio <= 0)
      throw new DomainException("Valor do prêmio deve ser maior que zero.");

    if (dataInicio >= dataFim)
      throw new DomainException("Data de início deve ser anterior à data de término.");

    Id = Guid.NewGuid();
    Numero = numero;
    ClienteId = clienteId;
    Placa = placa;
    ValorPremio = valorPremio;
    DataInicio = dataInicio;
    DataFim = dataFim;
    Status = StatusApolice.Ativa;
  }

  public void Cancelar()
  {
    if (Status == StatusApolice.Expirada)
      throw new DomainException("Não é possível cancelar uma apólice expirada.");

    if (Status == StatusApolice.Cancelada)
      throw new DomainException("Apólice já está cancelada.");

    Status = StatusApolice.Cancelada;
  }

  public void Expirar()
  {
    if (Status == StatusApolice.Cancelada)
      throw new DomainException("Não é possível expirar uma apólice cancelada.");

    Status = StatusApolice.Expirada;
  }

  public bool EstaVencendoEm(int dias)
  {
    var limite = DateTime.UtcNow.Date.AddDays(dias);
    return Status == StatusApolice.Ativa && DataFim.Date <= limite;
  }
}
