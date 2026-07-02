namespace InsurancePolicyManager.Application.DTOs;

public class ApoliceDto
{
  public Guid Id { get; set; }
  public string Numero { get; set; } = string.Empty;
  public ClienteDto Cliente { get; set; } = new();
  public string Placa { get; set; } = string.Empty;
  public decimal ValorPremio { get; set; }
  public DateTime DataInicio { get; set; }
  public DateTime DataFim { get; set; }
  public string Status { get; set; } = string.Empty;
}
