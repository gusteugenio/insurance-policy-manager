using InsurancePolicyManager.Application.Common;
using InsurancePolicyManager.Application.DTOs;
using InsurancePolicyManager.Application.Interfaces;
using InsurancePolicyManager.Domain.Entities;
using InsurancePolicyManager.Domain.Enums;
using InsurancePolicyManager.Domain.Exceptions;
using InsurancePolicyManager.Domain.Interfaces;

namespace InsurancePolicyManager.Application.Services;

public class ApoliceService : IApoliceService
{
  private readonly IApoliceRepository _apoliceRepository;
  private readonly IClienteRepository _clienteRepository;
  private readonly IPolicyNumberGenerator _numeroGenerator;

  public ApoliceService(
    IApoliceRepository apoliceRepository,
    IClienteRepository clienteRepository,
    IPolicyNumberGenerator numeroGenerator)
  {
    _apoliceRepository = apoliceRepository;
    _clienteRepository = clienteRepository;
    _numeroGenerator = numeroGenerator;
  }

  public async Task<ApoliceDto> CriarAsync(CriarApoliceDto dto, CancellationToken cancellationToken = default)
  {
    // Resolução automática de cliente: usa o existente ou cria um novo
    var cliente = await _clienteRepository.ObterPorDocumentoAsync(dto.DocumentoCliente, cancellationToken);
    if (cliente is null)
    {
      cliente = new Cliente(dto.DocumentoCliente, dto.NomeCliente);
      await _clienteRepository.AdicionarAsync(cliente, cancellationToken);
    }

    var numero = await _numeroGenerator.GerarNumeroAsync(cancellationToken);

    var apolice = new Apolice(numero, cliente.Id, dto.Placa, dto.ValorPremio, dto.DataInicio, dto.DataFim);
    await _apoliceRepository.AdicionarAsync(apolice, cancellationToken);

    apolice = await _apoliceRepository.ObterPorIdAsync(apolice.Id, cancellationToken);
    return MapearParaDto(apolice!);
  }

  public async Task<PagedResult<ApoliceDto>> ListarAsync(int pagina, int tamanhoPagina, string? status, string? ordenarPor, CancellationToken cancellationToken = default)
  {
    StatusApolice? statusEnum = null;
    if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<StatusApolice>(status, true, out var parsed))
      statusEnum = parsed;

    var (itens, total) = await _apoliceRepository.ListarAsync(pagina, tamanhoPagina, statusEnum, ordenarPor, cancellationToken);

    return new PagedResult<ApoliceDto>
    {
      Itens = itens.Select(MapearParaDto),
      Pagina = pagina,
      TamanhoPagina = tamanhoPagina,
      Total = total
    };
  }

  public async Task<ApoliceDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var apolice = await _apoliceRepository.ObterPorIdAsync(id, cancellationToken);
    return apolice is null ? null : MapearParaDto(apolice);
  }

  public async Task<ApoliceDto> AtualizarAsync(Guid id, AtualizarApoliceDto dto, CancellationToken cancellationToken = default)
  {
    var apolice = await _apoliceRepository.ObterPorIdAsync(id, cancellationToken)
      ?? throw new DomainException("Apólice não encontrada.");

    apolice.Atualizar(dto.Placa, dto.ValorPremio, dto.DataInicio, dto.DataFim);

    await _apoliceRepository.AtualizarAsync(apolice, cancellationToken);
    return MapearParaDto(apolice);
  }

  public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var apolice = await _apoliceRepository.ObterPorIdAsync(id, cancellationToken)
      ?? throw new DomainException("Apólice não encontrada.");

    await _apoliceRepository.RemoverAsync(apolice, cancellationToken);
  }

  public async Task<IEnumerable<ApoliceDto>> ListarVencendoEmAsync(int dias, CancellationToken cancellationToken = default)
  {
    var apolices = await _apoliceRepository.ListarVencendoEmAsync(dias, cancellationToken);
    return apolices.Select(MapearParaDto);
  }

  private static ApoliceDto MapearParaDto(Apolice apolice) => new()
  {
    Id = apolice.Id,
    Numero = apolice.Numero,
    Cliente = new DTOs.ClienteDto
    {
      Id = apolice.Cliente!.Id,
      Documento = apolice.Cliente.Documento,
      Nome = apolice.Cliente.Nome
    },
    Placa = apolice.Placa,
    ValorPremio = apolice.ValorPremio,
    DataInicio = apolice.DataInicio,
    DataFim = apolice.DataFim,
    Status = apolice.Status.ToString()
  };
}
