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

  [HttpGet("{documento}")]
  public async Task<IActionResult> ObterPorDocumento(string documento, CancellationToken cancellationToken)
  {
    var cliente = await _clienteService.ObterPorDocumentoAsync(documento, cancellationToken);
    if (cliente is null)
      return NotFound(ApiResponse<ClienteDto>.Fail("Cliente não encontrado."));

    return Ok(ApiResponse<ClienteDto>.Ok(cliente));
  }
}
