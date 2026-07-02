using InsurancePolicyManager.Application.DTOs;
using InsurancePolicyManager.Application.Interfaces;
using InsurancePolicyManager.Domain.Interfaces;

namespace InsurancePolicyManager.Application.Services;

public class ClienteService : IClienteService
{
  private readonly IClienteRepository _clienteRepository;

  public ClienteService(IClienteRepository clienteRepository) => _clienteRepository = clienteRepository;

  public async Task<ClienteDto?> ObterPorDocumentoAsync(string documento, CancellationToken cancellationToken = default)
  {
    var cliente = await _clienteRepository.ObterPorDocumentoAsync(documento, cancellationToken);
    return cliente is null ? null : new ClienteDto { Id = cliente.Id, Documento = cliente.Documento, Nome = cliente.Nome };
  }
}
