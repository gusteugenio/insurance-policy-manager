import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { Subscription, interval } from 'rxjs';
import { ApoliceService } from '../../../core/services/apolice.service';
import { Apolice, StatusApolice } from '../../../core/models/apolice.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-apolice-listagem',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatSlideToggleModule,
    LoadingSpinnerComponent
  ],
  templateUrl: './apolice-listagem.component.html',
  styleUrl: './apolice-listagem.component.scss'
})
export class ApoliceListagemComponent implements OnInit, OnDestroy {
  apolices: Apolice[] = [];
  total = 0;
  pagina = 1;
  tamanhoPagina = 10;
  carregando = false;

  statusSelecionado: StatusApolice | null = null;
  ordenacaoSelecionada: 'datainicio' | 'datafim' | 'valorpremio' | null = null;
  clienteId: string | null = null;
  vencendoEm30Dias = false;

  colunas = ['numero', 'dataInicio', 'cliente', 'placa', 'valorPremio', 'dataFim', 'status'];
  statusOptions: StatusApolice[] = ['Ativa', 'Cancelada', 'Expirada'];

  private pollingSubscription?: Subscription;

  constructor(
    private apoliceService: ApoliceService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.clienteId = this.route.snapshot.queryParamMap.get('clienteId');
    this.carregar();
    this.pollingSubscription = interval(20000).subscribe(() => this.carregar(true));
  }

  ngOnDestroy(): void {
    this.pollingSubscription?.unsubscribe();
  }

  carregar(silencioso = false): void {
    if (!silencioso) this.carregando = true;

    if (this.vencendoEm30Dias) {
      this.apoliceService.listarVencimentoProximo(30).subscribe({
        next: (resposta) => {
          this.apolices = resposta.data ?? [];
          this.total = this.apolices.length;
          this.carregando = false;
        },
        error: () => (this.carregando = false)
      });
      return;
    }

    this.apoliceService
      .listar({
        pagina: this.pagina,
        tamanhoPagina: this.tamanhoPagina,
        status: this.statusSelecionado ?? undefined,
        clienteId: this.clienteId ?? undefined,
        ordenarPor: this.ordenacaoSelecionada ?? undefined
      })
      .subscribe({
        next: (resposta) => {
          this.apolices = resposta.data?.itens ?? [];
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

  aoMudarFiltro(): void {
    this.pagina = 1;
    this.carregar();
  }

  aoAlternarVencendoEm30Dias(): void {
    this.pagina = 1;
    this.carregar();
  }

  limparFiltroCliente(): void {
    this.router.navigate(['/clientes']);
  }

  verDetalhe(apolice: Apolice): void {
    this.router.navigate(['/apolices', apolice.id]);
  }
}
