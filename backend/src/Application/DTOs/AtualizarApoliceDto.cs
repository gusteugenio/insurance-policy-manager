namespace InsurancePolicyManager.Application.DTOs;

public class AtualizarApoliceDto
{
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
