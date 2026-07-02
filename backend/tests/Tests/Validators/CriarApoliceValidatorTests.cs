using InsurancePolicyManager.Application.DTOs;
using InsurancePolicyManager.Application.Validators;
using Xunit;

namespace InsurancePolicyManager.Tests.Validators;

public class CriarApoliceValidatorTests
{
  private readonly CriarApoliceValidator _validator = new();

  // CPF e CNPJ válidos, usados apenas como massa de teste (não pertencem a pessoas reais)
  private static CriarApoliceDto CriarDtoValido() => new()
  {
    DocumentoCliente = "52998224725",
    NomeCliente = "João da Silva",
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

  [Theory]
  [InlineData("11111111111")]
  [InlineData("12345678900")]
  [InlineData("123")]
  public void Validate_ComCpfInvalido_DeveRetornarErro(string documentoInvalido)
  {
    var dto = CriarDtoValido();
    dto.DocumentoCliente = documentoInvalido;

    var resultado = _validator.Validate(dto);

    Assert.False(resultado.IsValid);
    Assert.Contains(resultado.Errors, e => e.PropertyName == nameof(CriarApoliceDto.DocumentoCliente));
  }

  [Fact]
  public void Validate_ComCnpjValido_NaoDeveRetornarErroDeDocumento()
  {
    var dto = CriarDtoValido();
    dto.DocumentoCliente = "11222333000181";

    var resultado = _validator.Validate(dto);

    Assert.DoesNotContain(resultado.Errors, e => e.PropertyName == nameof(CriarApoliceDto.DocumentoCliente));
  }

  [Theory]
  [InlineData("AB1234")]
  [InlineData("ABCD123")]
  [InlineData("123ABCD")]
  public void Validate_ComPlacaInvalida_DeveRetornarErro(string placaInvalida)
  {
    var dto = CriarDtoValido();
    dto.Placa = placaInvalida;

    var resultado = _validator.Validate(dto);

    Assert.Contains(resultado.Errors, e => e.PropertyName == nameof(CriarApoliceDto.Placa));
  }

  [Theory]
  [InlineData("ABC1234")]
  [InlineData("ABC1D23")]
  public void Validate_ComPlacaValidaAntigaOuMercosul_NaoDeveRetornarErroDePlaca(string placaValida)
  {
    var dto = CriarDtoValido();
    dto.Placa = placaValida;

    var resultado = _validator.Validate(dto);

    Assert.DoesNotContain(resultado.Errors, e => e.PropertyName == nameof(CriarApoliceDto.Placa));
  }

  [Fact]
  public void Validate_ComValorPremioZeroOuNegativo_DeveRetornarErro()
  {
    var dto = CriarDtoValido();
    dto.ValorPremio = 0;

    var resultado = _validator.Validate(dto);

    Assert.Contains(resultado.Errors, e => e.PropertyName == nameof(CriarApoliceDto.ValorPremio));
  }

  [Fact]
  public void Validate_ComDataInicioMaiorQueDataFim_DeveRetornarErro()
  {
    var dto = CriarDtoValido();
    dto.DataInicio = DateTime.UtcNow.AddDays(10);
    dto.DataFim = DateTime.UtcNow;

    var resultado = _validator.Validate(dto);

    Assert.Contains(resultado.Errors, e => e.PropertyName == nameof(CriarApoliceDto.DataInicio));
  }
}
