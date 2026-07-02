using InsurancePolicyManager.Application.Common;
using InsurancePolicyManager.Application.DTOs;
using InsurancePolicyManager.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace InsurancePolicyManager.Api.Controllers;

[ApiController]
[Route("api/apolices")]
public class ApolicesController : ControllerBase
{
  private readonly IApoliceService _apoliceService;
  private readonly IValidator<CriarApoliceDto> _criarValidator;
  private readonly IValidator<AtualizarApoliceDto> _atualizarValidator;

  public ApolicesController(
    IApoliceService apoliceService,
    IValidator<CriarApoliceDto> criarValidator,
    IValidator<AtualizarApoliceDto> atualizarValidator)
  {
    _apoliceService = apoliceService;
    _criarValidator = criarValidator;
    _atualizarValidator = atualizarValidator;
  }

  [HttpPost]
  public async Task<IActionResult> Criar([FromBody] CriarApoliceDto dto, CancellationToken cancellationToken)
  {
    var validation = await _criarValidator.ValidateAsync(dto, cancellationToken);
    if (!validation.IsValid)
      return BadRequest(ApiResponse<object>.Fail("Erro de validação.", validation.Errors.Select(e => e.ErrorMessage)));

    var apolice = await _apoliceService.CriarAsync(dto, cancellationToken);
    return CreatedAtAction(nameof(ObterPorId), new { id = apolice.Id }, ApiResponse<ApoliceDto>.Ok(apolice));
  }

  [HttpGet]
  public async Task<IActionResult> Listar(
    [FromQuery] int pagina = 1,
    [FromQuery] int tamanhoPagina = 10,
    [FromQuery] string? status = null,
    [FromQuery] string? ordenarPor = null,
    CancellationToken cancellationToken = default)
  {
    var erros = new List<string>();

    if (pagina < 1)
      erros.Add("O parâmetro 'pagina' deve ser maior ou igual a 1.");

    if (tamanhoPagina < 1 || tamanhoPagina > 100)
      erros.Add("O parâmetro 'tamanhoPagina' deve estar entre 1 e 100.");

    if (erros.Count > 0)
      return BadRequest(ApiResponse<object>.Fail("Erro de validação.", erros));

    var resultado = await _apoliceService.ListarAsync(pagina, tamanhoPagina, status, ordenarPor, cancellationToken);
    return Ok(ApiResponse<PagedResult<ApoliceDto>>.Ok(resultado));
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> ObterPorId(Guid id, CancellationToken cancellationToken)
  {
    var apolice = await _apoliceService.ObterPorIdAsync(id, cancellationToken);
    if (apolice is null)
      return NotFound(ApiResponse<ApoliceDto>.Fail("Apólice não encontrada."));

    return Ok(ApiResponse<ApoliceDto>.Ok(apolice));
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarApoliceDto dto, CancellationToken cancellationToken)
  {
    var validation = await _atualizarValidator.ValidateAsync(dto, cancellationToken);
    if (!validation.IsValid)
      return BadRequest(ApiResponse<object>.Fail("Erro de validação.", validation.Errors.Select(e => e.ErrorMessage)));

    var apolice = await _apoliceService.AtualizarAsync(id, dto, cancellationToken);
    return Ok(ApiResponse<ApoliceDto>.Ok(apolice));
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Remover(Guid id, CancellationToken cancellationToken)
  {
    await _apoliceService.RemoverAsync(id, cancellationToken);
    return NoContent();
  }

  [HttpGet("vencimento-proximo")]
  public async Task<IActionResult> VencimentoProximo([FromQuery] int dias = 30, CancellationToken cancellationToken = default)
  {
    if (dias < 0)
      return BadRequest(ApiResponse<object>.Fail("O parâmetro 'dias' não pode ser negativo."));

    var apolices = await _apoliceService.ListarVencendoEmAsync(dias, cancellationToken);
    return Ok(ApiResponse<IEnumerable<ApoliceDto>>.Ok(apolices));
  }
}
