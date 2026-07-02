using InsurancePolicyManager.Application.Services;
using InsurancePolicyManager.Domain.Entities;
using InsurancePolicyManager.Domain.Interfaces;
using Moq;
using Xunit;

namespace InsurancePolicyManager.Tests.Application;

public class ClienteServiceTests
{
  private readonly Mock<IClienteRepository> _clienteRepositoryMock = new();
  private readonly ClienteService _service;

  public ClienteServiceTests()
  {
    _service = new ClienteService(_clienteRepositoryMock.Object);
  }

  [Fact]
  public async Task ObterPorDocumentoAsync_QuandoClienteExiste_DeveRetornarDto()
  {
    var cliente = new Cliente("12345678900", "João da Silva");

    _clienteRepositoryMock
      .Setup(x => x.ObterPorDocumentoAsync("12345678900", It.IsAny<CancellationToken>()))
      .ReturnsAsync(cliente);

    var resultado = await _service.ObterPorDocumentoAsync("12345678900");

    Assert.NotNull(resultado);
    Assert.Equal("João da Silva", resultado!.Nome);
  }

  [Fact]
  public async Task ObterPorDocumentoAsync_QuandoClienteNaoExiste_DeveRetornarNull()
  {
    _clienteRepositoryMock
      .Setup(x => x.ObterPorDocumentoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((Cliente?)null);

    var resultado = await _service.ObterPorDocumentoAsync("00000000000");

    Assert.Null(resultado);
  }

  [Fact]
  public async Task ListarAsync_DeveRetornarPaginaComTotalCorreto()
  {
    var clientes = new List<Cliente>
    {
      new("12345678900", "João da Silva"),
      new("98765432100", "Maria Oliveira")
    };

    _clienteRepositoryMock
      .Setup(x => x.ListarAsync(1, 10, It.IsAny<CancellationToken>()))
      .ReturnsAsync((clientes, 2));

    var resultado = await _service.ListarAsync(1, 10);

    Assert.Equal(2, resultado.Total);
    Assert.Equal(2, resultado.Itens.Count());
  }
}
