# Fluxo funcional da aplicação

## 1. Cadastro de apólice

1. Usuário acessa a listagem de apólices (`/apolices`) e clica em "Nova apólice".
2. Preenche o formulário: documento do cliente, nome do cliente, placa, valor do prêmio, data de início e data de término.
3. O frontend valida os campos espelhando as regras do backend e envia `POST /api/apolices`.
4. O backend resolve o cliente: reutiliza um cliente existente pelo documento informado ou cria um novo automaticamente com os dados do formulário.
5. Gera o número da apólice (`SEG-YYYY-XXXX`) e persiste a apólice com status `Ativa`.
6. O frontend recebe a apólice criada e redireciona para a listagem/detalhe.

Ver diagrama de sequência em [`docs/diagrams/fluxo-cadastro.mmd`](diagrams/fluxo-cadastro.mmd).

## 2. Listagem e consulta

1. A tela `/apolices` consulta `GET /api/apolices` com paginação, podendo filtrar por status, por cliente (`clienteId`, recebido via navegação a partir da tela de clientes) e ordenar por data de cadastro (padrão), data de início, data de término ou valor do prêmio.
2. Um toggle "vencendo em 30 dias" alterna a consulta para `GET /api/apolices/vencimento-proximo?dias=30`, sempre fixo em 30 dias no frontend.
3. A listagem faz polling automático a cada 20 segundos, refletindo expirações feitas pelo job em background sem exigir refresh manual.
4. Ao clicar em uma apólice, o usuário é levado à tela de detalhe (`GET /api/apolices/{id}`).

## 3. Clientes

1. A tela `/clientes` consulta `GET /api/clientes` (paginado).
2. Ao selecionar um cliente, o usuário navega para a listagem de apólices filtrada por aquele cliente, reaproveitando a tela de apólices com o parâmetro `clienteId`.

## 4. Atualização de apólice

1. Na tela de edição, o usuário altera placa, valor do prêmio ou vigência.
2. O frontend envia `PUT /api/apolices/{id}`. Cliente e status não são afetados por esse fluxo.

## 5. Cancelamento

1. O usuário aciona o cancelamento manual de uma apólice `Ativa` (`PATCH /api/apolices/{id}/cancelar`).
2. O backend valida se a apólice está `Ativa`; caso já esteja `Cancelada` ou `Expirada`, retorna erro de negócio.

## 6. Expiração automática

1. O `ExpirarApolicesJob` (`BackgroundService`) roda a cada 1 minuto verificando apólices `Ativas` cuja `DataFim` já passou.
2. Essas apólices são marcadas automaticamente como `Expirada`, sem qualquer intervenção do usuário.

Ver diagrama de estados em [`docs/diagrams/transicao-status.mmd`](diagrams/transicao-status.mmd).

## 7. Exclusão

1. `DELETE /api/apolices/{id}` remove definitivamente uma apólice - fluxo administrativo, sem tela dedicada de confirmação de regra de negócio além da existência do registro.
