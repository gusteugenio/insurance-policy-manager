import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import { Cliente } from '../models/cliente.model';

@Injectable({ providedIn: 'root' })
export class ClienteService {
  private readonly baseUrl = '/clientes';

  constructor(private http: HttpClient) {}

  listar(pagina = 1, tamanhoPagina = 10): Observable<ApiResponse<PagedResult<Cliente>>> {
    const httpParams = new HttpParams()
      .set('pagina', pagina)
      .set('tamanhoPagina', tamanhoPagina);

    return this.http.get<ApiResponse<PagedResult<Cliente>>>(this.baseUrl, { params: httpParams });
  }

  obterPorDocumento(documento: string): Observable<ApiResponse<Cliente>> {
    return this.http.get<ApiResponse<Cliente>>(`${this.baseUrl}/${documento}`);
  }
}
