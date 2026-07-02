using InsurancePolicyManager.Application.DTOs;
using InsurancePolicyManager.Application.Validators;
using Xunit;

namespace InsurancePolicyManager.Tests.Validators;

public class AtualizarApoliceValidatorTests
{
  private readonly AtualizarApoliceValidator _validator = new();

  private static AtualizarApoliceDto CriarDtoValido() => new()
  {
    Placa = "ABC1D23",
    ValorPremio = 150m,
    DataInicio = DateTime.UtcNow,
    DataFim = DateTime.UtcNow.AddMonths(12)
  };

  [Fact]
  public void Validate_ComDadosValidos_NaoDeveRetornarErros()
  {
    var resultado = _validator.Validate(CriarDtoValido());

    Assert.True(resultado.IsValid);
  }

  [Fact]
  public void Validate_ComPlacaVazia_DeveRetornarErro()
  {
    var dto = CriarDtoValido();
    dto.Placa = "";

    var resultado = _validator.Validate(dto);

    Assert.Contains(resultado.Errors, e => e.PropertyName == nameof(AtualizarApoliceDto.Placa));
  }

  [Fact]
  public void Validate_ComValorPremioNegativo_DeveRetornarErro()
  {
    var dto = CriarDtoValido();
    dto.ValorPremio = -50m;

    var resultado = _validator.Validate(dto);

    Assert.Contains(resultado.Errors, e => e.PropertyName == nameof(AtualizarApoliceDto.ValorPremio));
  }

  [Fact]
  public void Validate_ComDataInicioMaiorQueDataFim_DeveRetornarErro()
  {
    var dto = CriarDtoValido();
    dto.DataInicio = DateTime.UtcNow.AddDays(10);
    dto.DataFim = DateTime.UtcNow;

    var resultado = _validator.Validate(dto);

    Assert.Contains(resultado.Errors, e => e.PropertyName == nameof(AtualizarApoliceDto.DataInicio));
  }
}
