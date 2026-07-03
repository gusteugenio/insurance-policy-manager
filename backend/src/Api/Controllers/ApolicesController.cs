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

  /// <summary>
  /// Cadastra uma nova apólice de seguro automóvel.
  /// </summary>
  /// <remarks>
  /// Se o cliente informado (por CPF/CNPJ) ainda não existir, ele é criado
  /// automaticamente com os dados enviados na requisição.
  /// </remarks>
  /// <response code="201">Apólice criada com sucesso.</response>
  /// <response code="400">Dados inválidos (documento, placa, datas ou valor).</response>
  [HttpPost]
  [ProducesResponseType(typeof(ApiResponse<ApoliceDto>), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Criar([FromBody] CriarApoliceDto dto, CancellationToken cancellationToken)
  {
    var validation = await _criarValidator.ValidateAsync(dto, cancellationToken);
    if (!validation.IsValid)
      return BadRequest(ApiResponse<object>.Fail("Erro de validação.", validation.Errors.Select(e => e.ErrorMessage)));

    var apolice = await _apoliceService.CriarAsync(dto, cancellationToken);
    return CreatedAtAction(nameof(ObterPorId), new { id = apolice.Id }, ApiResponse<ApoliceDto>.Ok(apolice));
  }

  /// <summary>
  /// Lista apólices cadastradas, com suporte a paginação, filtro por status e cliente, e ordenação.
  /// </summary>
  /// <param name="pagina">Número da página (mínimo 1).</param>
  /// <param name="tamanhoPagina">Quantidade de itens por página (entre 1 e 100).</param>
  /// <param name="status">Filtro opcional por status: Ativa, Cancelada ou Expirada.</param>
  /// <param name="clienteId">Filtro opcional para listar apenas apólices de um cliente específico.</param>
  /// <param name="ordenarPor">Campo de ordenação: dataInicio, dataFim ou valorPremio (padrão: dataInicio, decrescente).</param>
  /// <response code="200">Lista retornada com sucesso.</response>
  /// <response code="400">Parâmetros de paginação inválidos.</response>
  [HttpGet]
  [ProducesResponseType(typeof(ApiResponse<PagedResult<ApoliceDto>>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Listar(
    [FromQuery] int pagina = 1,
    [FromQuery] int tamanhoPagina = 10,
    [FromQuery] string? status = null,
    [FromQuery] Guid? clienteId = null,
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

    var resultado = await _apoliceService.ListarAsync(pagina, tamanhoPagina, status, clienteId, ordenarPor, cancellationToken);
    return Ok(ApiResponse<PagedResult<ApoliceDto>>.Ok(resultado));
  }

  /// <summary>
  /// Consulta uma apólice específica pelo Id.
  /// </summary>
  /// <param name="id">Identificador único da apólice.</param>
  /// <response code="200">Apólice encontrada.</response>
  /// <response code="404">Apólice não encontrada.</response>
  [HttpGet("{id:guid}")]
  [ProducesResponseType(typeof(ApiResponse<ApoliceDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> ObterPorId(Guid id, CancellationToken cancellationToken)
  {
    var apolice = await _apoliceService.ObterPorIdAsync(id, cancellationToken);
    if (apolice is null)
      return NotFound(ApiResponse<ApoliceDto>.Fail("Apólice não encontrada."));

    return Ok(ApiResponse<ApoliceDto>.Ok(apolice));
  }

  /// <summary>
  /// Atualiza os dados de uma apólice existente (placa, valor do prêmio e vigência).
  /// </summary>
  /// <param name="id">Identificador único da apólice.</param>
  /// <response code="200">Apólice atualizada com sucesso.</response>
  /// <response code="400">Dados inválidos.</response>
  /// <response code="404">Apólice não encontrada.</response>
  [HttpPut("{id:guid}")]
  [ProducesResponseType(typeof(ApiResponse<ApoliceDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarApoliceDto dto, CancellationToken cancellationToken)
  {
    var validation = await _atualizarValidator.ValidateAsync(dto, cancellationToken);
    if (!validation.IsValid)
      return BadRequest(ApiResponse<object>.Fail("Erro de validação.", validation.Errors.Select(e => e.ErrorMessage)));

    var apolice = await _apoliceService.AtualizarAsync(id, dto, cancellationToken);
    return Ok(ApiResponse<ApoliceDto>.Ok(apolice));
  }

  /// <summary>
  /// Cancela uma apólice ativa.
  /// </summary>
  /// <param name="id">Identificador único da apólice.</param>
  /// <response code="200">Apólice cancelada com sucesso.</response>
  /// <response code="400">Apólice já cancelada ou expirada.</response>
  /// <response code="404">Apólice não encontrada.</response>
  [HttpPatch("{id:guid}/cancelar")]
  [ProducesResponseType(typeof(ApiResponse<ApoliceDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Cancelar(Guid id, CancellationToken cancellationToken)
  {
    var apolice = await _apoliceService.CancelarAsync(id, cancellationToken);
    return Ok(ApiResponse<ApoliceDto>.Ok(apolice));
  }

  /// <summary>
  /// Remove uma apólice pelo Id.
  /// </summary>
  /// <param name="id">Identificador único da apólice.</param>
  /// <response code="204">Apólice removida com sucesso.</response>
  /// <response code="404">Apólice não encontrada.</response>
  [HttpDelete("{id:guid}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Remover(Guid id, CancellationToken cancellationToken)
  {
    await _apoliceService.RemoverAsync(id, cancellationToken);
    return NoContent();
  }

  /// <summary>
  /// Lista apólices ativas que vencem dentro do período informado.
  /// </summary>
  /// <param name="dias">Quantidade de dias a partir de hoje (padrão: 30).</param>
  /// <response code="200">Lista retornada com sucesso.</response>
  /// <response code="400">Parâmetro 'dias' inválido.</response>
  [HttpGet("vencimento-proximo")]
  [ProducesResponseType(typeof(ApiResponse<IEnumerable<ApoliceDto>>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> VencimentoProximo([FromQuery] int dias = 30, CancellationToken cancellationToken = default)
  {
    if (dias < 0)
      return BadRequest(ApiResponse<object>.Fail("O parâmetro 'dias' não pode ser negativo."));

    var apolices = await _apoliceService.ListarVencendoEmAsync(dias, cancellationToken);
    return Ok(ApiResponse<IEnumerable<ApoliceDto>>.Ok(apolices));
  }
}
