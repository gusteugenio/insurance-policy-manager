namespace InsurancePolicyManager.Application.DTOs;

public class ClienteDto
{
  public Guid Id { get; set; }
  public string Documento { get; set; } = string.Empty;
  public string Nome { get; set; } = string.Empty;
}
