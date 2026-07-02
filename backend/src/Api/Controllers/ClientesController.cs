using InsurancePolicyManager.Application.Common;
using InsurancePolicyManager.Application.DTOs;
using InsurancePolicyManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InsurancePolicyManager.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
  private readonly IClienteService _clienteService;

  public ClientesController(IClienteService clienteService) => _clienteService = clienteService;

  /// <summary>
  /// Consulta um cliente pelo CPF ou CNPJ.
  /// </summary>
  /// <param name="documento">CPF ou CNPJ do cliente (com ou sem formatação).</param>
  /// <response code="200">Cliente encontrado.</response>
  /// <response code="404">Cliente não encontrado.</response>
  [HttpGet("{documento}")]
  [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> ObterPorDocumento(string documento, CancellationToken cancellationToken)
  {
    var cliente = await _clienteService.ObterPorDocumentoAsync(documento, cancellationToken);
    if (cliente is null)
      return NotFound(ApiResponse<ClienteDto>.Fail("Cliente não encontrado."));

    return Ok(ApiResponse<ClienteDto>.Ok(cliente));
  }
}
