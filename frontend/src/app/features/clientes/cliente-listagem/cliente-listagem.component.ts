import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ClienteService } from '../../../core/services/cliente.service';
import { Cliente } from '../../../core/models/cliente.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-cliente-listagem',
  standalone: true,
  imports: [MatTableModule, MatPaginatorModule, MatButtonModule, MatIconModule, LoadingSpinnerComponent],
  templateUrl: './cliente-listagem.component.html',
  styleUrl: './cliente-listagem.component.scss'
})
export class ClienteListagemComponent implements OnInit {
  clientes: Cliente[] = [];
  total = 0;
  pagina = 1;
  tamanhoPagina = 10;
  carregando = false;

  colunas = ['nome', 'documento', 'acoes'];

  constructor(private clienteService: ClienteService, private router: Router) {}

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.carregando = true;
    this.clienteService.listar(this.pagina, this.tamanhoPagina).subscribe({
      next: (resposta) => {
        this.clientes = resposta.data?.itens ?? [];
        this.total = resposta.data?.total ?? 0;
        this.carregando = false;
      },
      error: () => (this.carregando = false)
    });
  }

  aoMudarPagina(evento: PageEvent): void {
    this.pagina = evento.pageIndex + 1;
    this.tamanhoPagina = evento.pageSize;
    this.carregar();
  }

  verApolices(cliente: Cliente): void {
    this.router.navigate(['/apolices'], { queryParams: { clienteId: cliente.id } });
  }
}
