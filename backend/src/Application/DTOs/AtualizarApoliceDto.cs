namespace InsurancePolicyManager.Application.DTOs;

public class AtualizarApoliceDto
{
  public string Placa { get; set; } = string.Empty;
  public decimal ValorPremio { get; set; }
  public DateTime DataInicio { get; set; }
  public DateTime DataFim { get; set; }
}
