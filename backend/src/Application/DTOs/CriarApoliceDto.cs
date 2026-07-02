namespace InsurancePolicyManager.Application.DTOs;

public class CriarApoliceDto
{
  public string DocumentoCliente { get; set; } = string.Empty;
  public string NomeCliente { get; set; } = string.Empty;
  public string Placa { get; set; } = string.Empty;
  public decimal ValorPremio { get; set; }
  public DateTime DataInicio { get; set; }
  public DateTime DataFim { get; set; }
}
