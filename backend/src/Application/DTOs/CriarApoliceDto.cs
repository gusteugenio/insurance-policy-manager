namespace InsurancePolicyManager.Application.DTOs;

public class CriarApoliceDto
{
  public string DocumentoCliente { get; set; } = string.Empty;
  public string NomeCliente { get; set; } = string.Empty;
  private string _placa = string.Empty;
  public string Placa
  {
    get => _placa;
    set => _placa = value?.Trim().ToUpperInvariant().Replace("-", "") ?? string.Empty;
  }
  public decimal ValorPremio { get; set; }
  public DateTime DataInicio { get; set; }
  public DateTime DataFim { get; set; }
}
