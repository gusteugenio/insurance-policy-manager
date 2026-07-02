using FluentValidation;
using InsurancePolicyManager.Application.DTOs;

namespace InsurancePolicyManager.Application.Validators;

public class AtualizarApoliceValidator : AbstractValidator<AtualizarApoliceDto>
{
  public AtualizarApoliceValidator()
  {
    RuleFor(x => x.Placa)
      .NotEmpty().WithMessage("Placa é obrigatória.")
      .Matches("^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$")
      .WithMessage("Placa inválida. Utilize o padrão antigo (ABC1234) ou Mercosul (ABC1D23).");

    RuleFor(x => x.ValorPremio)
      .GreaterThan(0).WithMessage("Valor do prêmio deve ser maior que zero.");

    RuleFor(x => x.DataInicio)
      .LessThan(x => x.DataFim).WithMessage("Data de início deve ser anterior à data de término.");
  }
}
