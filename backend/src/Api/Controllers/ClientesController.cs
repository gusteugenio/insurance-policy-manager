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
  /// Lista clientes cadastrados, com suporte a paginação.
  /// </summary>
  /// <param name="pagina">Número da página (mínimo 1).</param>
  /// <param name="tamanhoPagina">Quantidade de itens por página (entre 1 e 100).</param>
  /// <response code="200">Lista retornada com sucesso.</response>
  /// <response code="400">Parâmetros de paginação inválidos.</response>
  [HttpGet]
  [ProducesResponseType(typeof(ApiResponse<PagedResult<ClienteDto>>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Listar(
    [FromQuery] int pagina = 1,
    [FromQuery] int tamanhoPagina = 10,
    CancellationToken cancellationToken = default)
  {
    var erros = new List<string>();

    if (pagina < 1)
      erros.Add("O parâmetro 'pagina' deve ser maior ou igual a 1.");

    if (tamanhoPagina < 1 || tamanhoPagina > 100)
      erros.Add("O parâmetro 'tamanhoPagina' deve estar entre 1 e 100.");

    if (erros.Count > 0)
      return BadRequest(ApiResponse<object>.Fail("Erro de validação.", erros));

    var resultado = await _clienteService.ListarAsync(pagina, tamanhoPagina, cancellationToken);
    return Ok(ApiResponse<PagedResult<ClienteDto>>.Ok(resultado));
  }

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
