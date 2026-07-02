using InsurancePolicyManager.Domain.Entities;
using InsurancePolicyManager.Domain.Enums;
using InsurancePolicyManager.Domain.Exceptions;
using Xunit;

namespace InsurancePolicyManager.Tests.Domain;

public class ApoliceTests
{
  private static Apolice CriarApoliceValida()
    => new(
      "SEG-2026-0001",
      Guid.NewGuid(),
      "ABC1D23",
      150m,
      DateTime.UtcNow,
      DateTime.UtcNow.AddMonths(12));

  [Fact]
  public void Construtor_ComDadosValidos_DeveCriarApoliceComStatusAtiva()
  {
    var apolice = CriarApoliceValida();

    Assert.Equal(StatusApolice.Ativa, apolice.Status);
  }

  [Fact]
  public void Construtor_ComDataInicioMaiorQueDataFim_DeveLancarDomainException()
  {
    var dataInicio = DateTime.UtcNow;
    var dataFim = dataInicio.AddDays(-1);

    Assert.Throws<DomainException>(() =>
      new Apolice("SEG-2026-0001", Guid.NewGuid(), "ABC1D23", 150m, dataInicio, dataFim));
  }

  [Fact]
  public void Construtor_ComValorPremioZeroOuNegativo_DeveLancarDomainException()
  {
    Assert.Throws<DomainException>(() =>
      new Apolice("SEG-2026-0001", Guid.NewGuid(), "ABC1D23", 0m, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1)));
  }

  [Fact]
  public void Cancelar_QuandoApoliceEstaAtiva_DeveAlterarStatusParaCancelada()
  {
    var apolice = CriarApoliceValida();

    apolice.Cancelar();

    Assert.Equal(StatusApolice.Cancelada, apolice.Status);
  }

  [Fact]
  public void Cancelar_QuandoApoliceJaEstaCancelada_DeveLancarDomainException()
  {
    var apolice = CriarApoliceValida();
    apolice.Cancelar();

    Assert.Throws<DomainException>(() => apolice.Cancelar());
  }

  [Fact]
  public void Cancelar_QuandoApoliceEstaExpirada_DeveLancarDomainException()
  {
    var apolice = CriarApoliceValida();
    apolice.Expirar();

    Assert.Throws<DomainException>(() => apolice.Cancelar());
  }

  [Fact]
  public void Expirar_QuandoApoliceEstaAtiva_DeveAlterarStatusParaExpirada()
  {
    var apolice = CriarApoliceValida();

    apolice.Expirar();

    Assert.Equal(StatusApolice.Expirada, apolice.Status);
  }

  [Fact]
  public void Expirar_QuandoApoliceEstaCancelada_DeveLancarDomainException()
  {
    var apolice = CriarApoliceValida();
    apolice.Cancelar();

    Assert.Throws<DomainException>(() => apolice.Expirar());
  }

  [Fact]
  public void EstaVencendoEm_QuandoApoliceAtivaEDentroDoPrazo_DeveRetornarTrue()
  {
    var apolice = new Apolice(
      "SEG-2026-0001",
      Guid.NewGuid(),
      "ABC1D23",
      150m,
      DateTime.UtcNow.AddMonths(-11),
      DateTime.UtcNow.AddDays(15));

    Assert.True(apolice.EstaVencendoEm(30));
  }

  [Fact]
  public void EstaVencendoEm_QuandoApoliceCancelada_DeveRetornarFalse()
  {
    var apolice = new Apolice(
      "SEG-2026-0001",
      Guid.NewGuid(),
      "ABC1D23",
      150m,
      DateTime.UtcNow.AddMonths(-11),
      DateTime.UtcNow.AddDays(15));
    apolice.Cancelar();

    Assert.False(apolice.EstaVencendoEm(30));
  }

  [Fact]
  public void Atualizar_ComDadosValidos_DeveAtualizarCamposDaApolice()
  {
    var apolice = CriarApoliceValida();

    apolice.Atualizar("XYZ9K88", 200m, DateTime.UtcNow, DateTime.UtcNow.AddMonths(24));

    Assert.Equal("XYZ9K88", apolice.Placa);
    Assert.Equal(200m, apolice.ValorPremio);
  }
}
