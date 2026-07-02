// src/Api/Controllers/ApolicesController.cs
using InsurancePolicyManager.Application.Common;
using InsurancePolicyManager.Application.DTOs;
using InsurancePolicyManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InsurancePolicyManager.Api.Controllers;

[ApiController]
[Route("api/apolices")]
public class ApolicesController : ControllerBase
{
    private readonly IApoliceService _apoliceService;

    public ApolicesController(IApoliceService apoliceService) => _apoliceService = apoliceService;

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarApoliceDto dto, CancellationToken cancellationToken)
    {
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
        var apolices = await _apoliceService.ListarVencendoEmAsync(dias, cancellationToken);
        return Ok(ApiResponse<IEnumerable<ApoliceDto>>.Ok(apolices));
    }
}
