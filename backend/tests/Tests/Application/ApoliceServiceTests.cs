using System.Reflection;
using InsurancePolicyManager.Application.DTOs;
using InsurancePolicyManager.Application.Services;
using InsurancePolicyManager.Domain.Entities;
using InsurancePolicyManager.Domain.Exceptions;
using InsurancePolicyManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using InsurancePolicyManager.Application.Common;

namespace InsurancePolicyManager.Tests.Application;

public class ApoliceServiceTests
{
  private readonly Mock<IApoliceRepository> _apoliceRepositoryMock = new();
  private readonly Mock<IClienteRepository> _clienteRepositoryMock = new();
  private readonly Mock<IPolicyNumberGenerator> _numeroGeneratorMock = new();
  private readonly Mock<ICorrelationIdProvider> _correlationIdProviderMock = new();
  private readonly Mock<ILogger<ApoliceService>> _loggerMock = new();
  private readonly ApoliceService _service;

  public ApoliceServiceTests()
  {
    _correlationIdProviderMock.Setup(x => x.GetCorrelationId()).Returns("test-correlation-id");
    _numeroGeneratorMock.Setup(x => x.GerarNumeroAsync(It.IsAny<CancellationToken>())).ReturnsAsync("SEG-2026-0001");

    _service = new ApoliceService(
      _apoliceRepositoryMock.Object,
      _clienteRepositoryMock.Object,
      _numeroGeneratorMock.Object,
      _correlationIdProviderMock.Object,
      _loggerMock.Object);
  }

  // Cliente.Cliente (navegação) tem setter privado, só populado pelo EF Core em runtime real.
  // Em teste de unidade, usamos reflexão para simular esse mesmo comportamento.
  private static void VincularCliente(Apolice apolice, Cliente cliente)
    => typeof(Apolice)
      .GetProperty(nameof(Apolice.Cliente), BindingFlags.Public | BindingFlags.Instance)!
      .SetValue(apolice, cliente);

  private static CriarApoliceDto CriarDtoValido() => new()
  {
    DocumentoCliente = "12345678900",
    NomeCliente = "João da Silva",
    Placa = "ABC1D23",
    ValorPremio = 150m,
    DataInicio = DateTime.UtcNow,
    DataFim = DateTime.UtcNow.AddMonths(12)
  };

  [Fact]
  public async Task CriarAsync_QuandoClienteNaoExiste_DeveCriarNovoClienteAutomaticamente()
  {
    _clienteRepositoryMock
      .Setup(x => x.ObterPorDocumentoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((Cliente?)null);

    Apolice? apoliceCriada = null;
    _apoliceRepositoryMock
      .Setup(x => x.AdicionarAsync(It.IsAny<Apolice>(), It.IsAny<CancellationToken>()))
      .Callback<Apolice, CancellationToken>((a, _) => apoliceCriada = a)
      .Returns(Task.CompletedTask);

    _apoliceRepositoryMock
      .Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(() =>
      {
        VincularCliente(apoliceCriada!, new Cliente("12345678900", "João da Silva"));
        return apoliceCriada;
      });

    await _service.CriarAsync(CriarDtoValido());

    _clienteRepositoryMock.Verify(
      x => x.AdicionarAsync(It.Is<Cliente>(c => c.Documento == "12345678900"), It.IsAny<CancellationToken>()),
      Times.Once);
  }

  [Fact]
  public async Task CriarAsync_QuandoClienteJaExiste_NaoDeveCriarNovoCliente()
  {
    var clienteExistente = new Cliente("12345678900", "João da Silva");

    _clienteRepositoryMock
      .Setup(x => x.ObterPorDocumentoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(clienteExistente);

    _apoliceRepositoryMock
      .Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((Guid id, CancellationToken _) =>
      {
        var apolice = new Apolice("SEG-2026-0001", clienteExistente.Id, "ABC1D23", 150m, DateTime.UtcNow, DateTime.UtcNow.AddMonths(12));
        VincularCliente(apolice, clienteExistente);
        return apolice;
      });

    await _service.CriarAsync(CriarDtoValido());

    _clienteRepositoryMock.Verify(
      x => x.AdicionarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()),
      Times.Never);
  }

  [Fact]
  public async Task RemoverAsync_QuandoApoliceNaoExiste_DeveLancarDomainException()
  {
    _apoliceRepositoryMock
      .Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((Apolice?)null);

    await Assert.ThrowsAsync<DomainException>(() => _service.RemoverAsync(Guid.NewGuid()));
  }

  [Fact]
  public async Task RemoverAsync_QuandoApoliceExiste_DeveChamarRepositorioDeRemocao()
  {
    var cliente = new Cliente("12345678900", "João da Silva");
    var apolice = new Apolice("SEG-2026-0001", cliente.Id, "ABC1D23", 150m, DateTime.UtcNow, DateTime.UtcNow.AddMonths(12));

    _apoliceRepositoryMock
      .Setup(x => x.ObterPorIdAsync(apolice.Id, It.IsAny<CancellationToken>()))
      .ReturnsAsync(apolice);

    await _service.RemoverAsync(apolice.Id);

    _apoliceRepositoryMock.Verify(x => x.RemoverAsync(apolice, It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  public async Task CancelarAsync_QuandoApoliceAtiva_DeveAlterarStatusParaCancelada()
  {
    var cliente = new Cliente("12345678900", "João da Silva");
    var apolice = new Apolice("SEG-2026-0001", cliente.Id, "ABC1D23", 150m, DateTime.UtcNow, DateTime.UtcNow.AddMonths(12));
    VincularCliente(apolice, cliente);

    _apoliceRepositoryMock
      .Setup(x => x.ObterPorIdAsync(apolice.Id, It.IsAny<CancellationToken>()))
      .ReturnsAsync(apolice);

    var resultado = await _service.CancelarAsync(apolice.Id);

    Assert.Equal(nameof(InsurancePolicyManager.Domain.Enums.StatusApolice.Cancelada), resultado.Status);
    _apoliceRepositoryMock.Verify(x => x.AtualizarAsync(apolice, It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  public async Task CancelarAsync_QuandoApoliceNaoExiste_DeveLancarDomainException()
  {
    _apoliceRepositoryMock
      .Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((Apolice?)null);

    await Assert.ThrowsAsync<DomainException>(() => _service.CancelarAsync(Guid.NewGuid()));
  }

  [Fact]
  public async Task ListarAsync_ComClienteIdInformado_DevePassarFiltroParaORepositorio()
  {
    var clienteId = Guid.NewGuid();

    _apoliceRepositoryMock
      .Setup(x => x.ListarAsync(1, 10, null, clienteId, null, It.IsAny<CancellationToken>()))
      .ReturnsAsync((new List<Apolice>(), 0));

    await _service.ListarAsync(1, 10, null, clienteId, null);

    _apoliceRepositoryMock.Verify(
      x => x.ListarAsync(1, 10, null, clienteId, null, It.IsAny<CancellationToken>()),
      Times.Once);
  }

  [Fact]
  public async Task ListarAsync_SemClienteIdInformado_DevePassarNullParaORepositorio()
  {
    _apoliceRepositoryMock
      .Setup(x => x.ListarAsync(1, 10, null, null, null, It.IsAny<CancellationToken>()))
      .ReturnsAsync((new List<Apolice>(), 0));

    await _service.ListarAsync(1, 10, null, null, null);

    _apoliceRepositoryMock.Verify(
      x => x.ListarAsync(1, 10, null, null, null, It.IsAny<CancellationToken>()),
      Times.Once);
  }
}
