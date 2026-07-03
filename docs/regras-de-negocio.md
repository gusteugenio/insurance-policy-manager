# Regras de negócio

## Cliente

- `Documento` (CPF ou CNPJ) é normalizado (somente dígitos) antes de ser persistido.
- CPF/CNPJ é validado por algoritmo de dígito verificador no momento do cadastro de uma apólice.
- `Nome` é obrigatório.
- **Resolução automática de cliente**: ao cadastrar uma apólice, o serviço busca um cliente pelo documento informado. Se não existir, o cliente é criado automaticamente com os dados enviados no próprio formulário de apólice, sem exigir um cadastro separado.

## Apólice

- `Numero` é gerado automaticamente no padrão `SEG-YYYY-XXXX` (ano corrente + sequencial), nunca informado pelo cliente da API.
- Toda apólice nasce com status `Ativa`.
- `Placa` é obrigatória e validada no formato antigo (`ABC1234`) ou Mercosul (`ABC1D23`).
- `ValorPremio` deve ser maior que zero.
- `DataInicio` deve ser anterior a `DataFim`.
- `DataCriacao` é preenchida automaticamente (UTC) no momento do cadastro e não pode ser alterada. Ela é conceitualmente distinta de `DataInicio`: representa quando o registro foi criado no sistema, não quando a vigência do seguro começa.

## Transições de status

- **Ativa → Cancelada**: acionada manualmente via `PATCH /api/apolices/{id}/cancelar`. Só é permitida se a apólice estiver `Ativa`; apólices `Canceladas` ou `Expiradas` não podem ser canceladas novamente (a tentativa lança `DomainException`).
- **Ativa → Expirada**: automática. Um `BackgroundService` (`ExpirarApolicesJob`) verifica periodicamente (a cada 1 minuto - em produção o ideal seria a cada 24h) as apólices `Ativas` cuja `DataFim` já foi ultrapassada e as marca como `Expirada`.
- Uma apólice `Cancelada` não pode ser expirada pelo job (lança exceção de domínio).
- `Cancelada` e `Expirada` são estados terminais: uma apólice nunca retorna ao status `Ativa`.

## Atualização de apólice

- `PUT /api/apolices/{id}` permite alterar apenas `Placa`, `ValorPremio`, `DataInicio` e `DataFim`.
- Cliente e status **não** são alterados por esse endpoint.
- As mesmas validações de criação (placa, valor, datas) se aplicam na atualização.

## Consulta de vencimento próximo

- Regra de domínio `EstaVencendoEm(dias)`: verdadeira quando a apólice está `Ativa` **e** `DataFim <= hoje + dias`.
- O endpoint `GET /api/apolices/vencimento-proximo` aceita o parâmetro livre `dias` (padrão `30`).
- No frontend, esse período é mantido **fixo em 30 dias**, seguindo a mesma regra de negócio usada para acompanhamento de vencimentos - consumidores externos da API continuam podendo informar qualquer valor.

## Validações de entrada (FluentValidation)

**Criação (`CriarApoliceDto`)**
- `DocumentoCliente`: obrigatório, CPF ou CNPJ válido (dígito verificador).
- `NomeCliente`: obrigatório.
- `Placa`: obrigatória, regex de placa antiga/Mercosul.
- `ValorPremio`: maior que zero.
- `DataInicio` < `DataFim`.

**Atualização (`AtualizarApoliceDto`)**
- `Placa`: obrigatória, mesma regex.
- `ValorPremio`: maior que zero.
- `DataInicio` < `DataFim`.

## Padronização de respostas

Toda resposta da API segue o formato `ApiResponse<T>`:

```json
{
  "success": true,
  "data": { },
  "message": null,
  "errors": null
}
```
