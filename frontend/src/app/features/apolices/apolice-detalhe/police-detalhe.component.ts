import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { finalize } from 'rxjs';

import { ApoliceService } from '../../../core/services/apolice.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { Apolice } from '../../../core/models/apolice.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-apolice-detalhe',
  standalone: true,
  imports: [CommonModule, RouterLink, MatButtonModule, MatIconModule, MatDialogModule, LoadingSpinnerComponent],
  templateUrl: './apolice-detalhe.component.html',
  styleUrl: './apolice-detalhe.component.scss'
})
export class ApoliceDetalheComponent implements OnInit {
  apolice: Apolice | null = null;
  carregando = false;
  processando = false;
  apoliceId!: string;

  constructor(
    private apoliceService: ApoliceService,
    private notification: NotificationService,
    private dialog: MatDialog,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.apoliceId = this.route.snapshot.paramMap.get('id')!;
    this.carregar();
  }

  carregar(): void {
    this.carregando = true;
    this.apoliceService
      .obterPorId(this.apoliceId)
      .pipe(finalize(() => (this.carregando = false)))
      .subscribe({
        next: (resposta) => (this.apolice = resposta.data),
        error: () => this.router.navigate(['/apolices'])
      });
  }

  cancelarApolice(): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: {
        titulo: 'Cancelar apólice',
        mensagem: `Tem certeza que deseja cancelar a apólice ${this.apolice?.numero}?`,
        textoConfirmar: 'Cancelar apólice'
      }
    });

    ref.afterClosed().subscribe((confirmado) => {
      if (!confirmado) return;

      this.processando = true;
      this.apoliceService
        .cancelar(this.apoliceId)
        .pipe(finalize(() => (this.processando = false)))
        .subscribe({
          next: (resposta) => {
            this.apolice = resposta.data;
            this.notification.sucesso('Apólice cancelada com sucesso.');
          }
        });
    });
  }

  removerApolice(): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: {
        titulo: 'Remover apólice',
        mensagem: `Tem certeza que deseja remover a apólice ${this.apolice?.numero}? Essa ação não pode ser desfeita.`,
        textoConfirmar: 'Remover'
      }
    });

    ref.afterClosed().subscribe((confirmado) => {
      if (!confirmado) return;

      this.processando = true;
      this.apoliceService
        .remover(this.apoliceId)
        .pipe(finalize(() => (this.processando = false)))
        .subscribe({
          next: () => {
            this.notification.sucesso('Apólice removida com sucesso.');
            this.router.navigate(['/apolices']);
          }
        });
    });
  }

  verApolicesDoCliente(): void {
    if (!this.apolice) return;
    this.router.navigate(['/apolices'], { queryParams: { clienteId: this.apolice.cliente.id } });
  }
}
