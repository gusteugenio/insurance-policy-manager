using InsurancePolicyManager.Domain.Exceptions;

namespace InsurancePolicyManager.Domain.Entities;

public class Cliente
{
  public Guid Id { get; private set; }
  public string Documento { get; private set; } = string.Empty;
  public string Nome { get; private set; } = string.Empty;

  private readonly List<Apolice> _apolices = new();
  public IReadOnlyCollection<Apolice> Apolices => _apolices.AsReadOnly();

  protected Cliente() { }  // uso exclusivo do EF Core

  public Cliente(string documento, string nome)
  {
    var documentoNormalizado = NormalizarDocumento(documento);

    if (string.IsNullOrWhiteSpace(documentoNormalizado))
      throw new DomainException("Documento do cliente é obrigatório.");

    if (string.IsNullOrWhiteSpace(nome))
      throw new DomainException("Nome do cliente é obrigatório.");

    Id = Guid.NewGuid();
    Documento = documentoNormalizado;
    Nome = nome;
  }

  public static string NormalizarDocumento(string documento)
    => new string((documento ?? string.Empty).Where(char.IsDigit).ToArray());
}
