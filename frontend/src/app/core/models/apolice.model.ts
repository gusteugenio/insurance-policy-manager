import { Cliente } from './cliente.model';

export type StatusApolice = 'Ativa' | 'Cancelada' | 'Expirada';

export interface Apolice {
  id: string;
  numero: string;
  cliente: Cliente;
  placa: string;
  valorPremio: number;
  dataInicio: string;
  dataFim: string;
  status: StatusApolice;
}

export interface CriarApoliceRequest {
  documentoCliente: string;
  nomeCliente: string;
  placa: string;
  valorPremio: number;
  dataInicio: string;
  dataFim: string;
}

export interface AtualizarApoliceRequest {
  placa: string;
  valorPremio: number;
  dataInicio: string;
  dataFim: string;
}

export interface ListarApolicesParams {
  pagina?: number;
  tamanhoPagina?: number;
  status?: StatusApolice;
  clienteId?: string;
  ordenarPor?: 'datainicio' | 'datafim' | 'valorpremio';
}
