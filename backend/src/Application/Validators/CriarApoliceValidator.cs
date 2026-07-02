using FluentValidation;
using InsurancePolicyManager.Application.DTOs;

namespace InsurancePolicyManager.Application.Validators;

public class CriarApoliceValidator : AbstractValidator<CriarApoliceDto>
{
  public CriarApoliceValidator()
  {
    RuleFor(x => x.DocumentoCliente)
      .NotEmpty().WithMessage("Documento do cliente é obrigatório.")
      .Must(DocumentoValido)
      .WithMessage("CPF ou CNPJ inválido.");

    RuleFor(x => x.NomeCliente)
      .NotEmpty().WithMessage("Nome do cliente é obrigatório.");

    RuleFor(x => x.Placa)
      .NotEmpty().WithMessage("Placa é obrigatória.")
      .Matches("^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$")
      .WithMessage("Placa inválida. Utilize o padrão antigo (ABC1234) ou Mercosul (ABC1D23).");

    RuleFor(x => x.ValorPremio)
      .GreaterThan(0).WithMessage("Valor do prêmio deve ser maior que zero.");

    RuleFor(x => x.DataInicio)
      .LessThan(x => x.DataFim).WithMessage("Data de início deve ser anterior à data de término.");
  }

  private static bool DocumentoValido(string documento)
  {
    if (string.IsNullOrWhiteSpace(documento))
      return false;

    documento = new string(documento.Where(char.IsDigit).ToArray());

    return documento.Length switch
    {
      11 => CpfValido(documento),
      14 => CnpjValido(documento),
      _ => false
    };
  }

  private static bool CpfValido(string cpf)
  {
    if (cpf.Distinct().Count() == 1)
      return false;

    int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
    int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

    var tempCpf = cpf[..9];
    var soma = tempCpf.Select((t, i) => (t - '0') * multiplicador1[i]).Sum();

    var resto = soma % 11;
    var digito = resto < 2 ? 0 : 11 - resto;

    tempCpf += digito;

    soma = tempCpf.Select((t, i) => (t - '0') * multiplicador2[i]).Sum();

    resto = soma % 11;
    digito = resto < 2 ? 0 : 11 - resto;

    return cpf.EndsWith($"{tempCpf[9]}{digito}");
  }

  private static bool CnpjValido(string cnpj)
  {
    if (cnpj.Distinct().Count() == 1)
      return false;

    int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
    int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

    var tempCnpj = cnpj[..12];
    var soma = tempCnpj.Select((t, i) => (t - '0') * multiplicador1[i]).Sum();

    var resto = soma % 11;
    var digito = resto < 2 ? 0 : 11 - resto;

    tempCnpj += digito;

    soma = tempCnpj.Select((t, i) => (t - '0') * multiplicador2[i]).Sum();

    resto = soma % 11;
    digito = resto < 2 ? 0 : 11 - resto;

    return cnpj.EndsWith($"{tempCnpj[12]}{digito}");
  }
}
