import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import {
  Apolice,
  AtualizarApoliceRequest,
  CriarApoliceRequest,
  ListarApolicesParams
} from '../models/apolice.model';

@Injectable({ providedIn: 'root' })
export class ApoliceService {
  private readonly baseUrl = '/apolices';

  constructor(private http: HttpClient) {}

  listar(params: ListarApolicesParams): Observable<ApiResponse<PagedResult<Apolice>>> {
    let httpParams = new HttpParams();

    if (params.pagina) httpParams = httpParams.set('pagina', params.pagina);
    if (params.tamanhoPagina) httpParams = httpParams.set('tamanhoPagina', params.tamanhoPagina);
    if (params.status) httpParams = httpParams.set('status', params.status);
    if (params.clienteId) httpParams = httpParams.set('clienteId', params.clienteId);
    if (params.ordenarPor) httpParams = httpParams.set('ordenarPor', params.ordenarPor);

    return this.http.get<ApiResponse<PagedResult<Apolice>>>(this.baseUrl, { params: httpParams });
  }

  obterPorId(id: string): Observable<ApiResponse<Apolice>> {
    return this.http.get<ApiResponse<Apolice>>(`${this.baseUrl}/${id}`);
  }

  criar(dto: CriarApoliceRequest): Observable<ApiResponse<Apolice>> {
    return this.http.post<ApiResponse<Apolice>>(this.baseUrl, dto);
  }

  atualizar(id: string, dto: AtualizarApoliceRequest): Observable<ApiResponse<Apolice>> {
    return this.http.put<ApiResponse<Apolice>>(`${this.baseUrl}/${id}`, dto);
  }

  remover(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  cancelar(id: string): Observable<ApiResponse<Apolice>> {
    return this.http.patch<ApiResponse<Apolice>>(`${this.baseUrl}/${id}/cancelar`, {});
  }

  listarVencimentoProximo(dias = 30): Observable<ApiResponse<Apolice[]>> {
    const httpParams = new HttpParams().set('dias', dias);
    return this.http.get<ApiResponse<Apolice[]>>(`${this.baseUrl}/vencimento-proximo`, { params: httpParams });
  }
}
