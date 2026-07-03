import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { Subject, Subscription, of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, finalize, map, switchMap } from 'rxjs/operators';

import { ApoliceService } from '../../../core/services/apolice.service';
import { ClienteService } from '../../../core/services/cliente.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import {
  dataInicioAntesDataFimValidator,
  documentoValidator,
  formatarDocumento,
  placaValidator,
  valorPremioValidator
} from '../../../shared/validators/apolice.validators';

@Component({
  selector: 'app-apolice-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    LoadingSpinnerComponent
  ],
  templateUrl: './apolice-form.component.html',
  styleUrl: './apolice-form.component.scss'
})
export class ApoliceFormComponent implements OnInit, OnDestroy {
  form!: FormGroup;
  modoEdicao = false;
  apoliceId: string | null = null;
  numeroApolice: string | null = null;
  clienteEncontrado = false;

  carregando = false;
  salvando = false;

  private documentoSubject = new Subject<string>();
  private documentoSubscription?: Subscription;

  constructor(
    private fb: FormBuilder,
    private apoliceService: ApoliceService,
    private clienteService: ClienteService,
    private notification: NotificationService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.apoliceId = this.route.snapshot.paramMap.get('id');
    this.modoEdicao = !!this.apoliceId;

    this.form = this.fb.group(
      {
        documentoCliente: ['', this.modoEdicao ? [] : [Validators.required, documentoValidator()]],
        nomeCliente: ['', this.modoEdicao ? [] : [Validators.required]],
        placa: ['', [Validators.required, placaValidator()]],
        valorPremio: [null, [Validators.required, valorPremioValidator()]],
        dataInicio: [null, [Validators.required]],
        dataFim: [null, [Validators.required]]
      },
      { validators: dataInicioAntesDataFimValidator() }
    );

    this.documentoSubscription = this.documentoSubject
      .pipe(
        debounceTime(400),
        distinctUntilChanged(),
        switchMap((documento) => {
          const digitos = documento.replace(/\D/g, '');
          if (digitos.length !== 11 && digitos.length !== 14) {
            return of(null);
          }
          return this.clienteService.obterPorDocumento(digitos).pipe(
            map((resposta) => resposta.data),
            catchError(() => of(null))
          );
        })
      )
      .subscribe((cliente) => {
        const nomeControl = this.form.get('nomeCliente');
        if (cliente) {
          this.clienteEncontrado = true;
          nomeControl?.setValue(cliente.nome);
          nomeControl?.disable();
        } else {
          this.clienteEncontrado = false;
          nomeControl?.enable();
        }
      });

    if (this.modoEdicao) {
      this.carregarApolice();
    }
  }

  ngOnDestroy(): void {
    this.documentoSubscription?.unsubscribe();
  }

  aoDigitarDocumento(event: Event): void {
    const input = event.target as HTMLInputElement;
    const formatado = formatarDocumento(input.value);
    input.value = formatado;
    this.form.get('documentoCliente')?.setValue(formatado, { emitEvent: false });
    this.documentoSubject.next(formatado);
  }

  private carregarApolice(): void {
    this.carregando = true;
    this.apoliceService
      .obterPorId(this.apoliceId!)
      .pipe(finalize(() => (this.carregando = false)))
      .subscribe({
        next: (resposta) => {
          const apolice = resposta.data;
          if (!apolice) return;

          this.numeroApolice = apolice.numero;
          this.form.patchValue({
            placa: apolice.placa,
            valorPremio: apolice.valorPremio,
            dataInicio: new Date(apolice.dataInicio),
            dataFim: new Date(apolice.dataFim)
          });
        },
        error: () => this.router.navigate(['/apolices'])
      });
  }

  salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.salvando = true;
    const valores = this.form.getRawValue();

    const payloadComum = {
      placa: valores.placa,
      valorPremio: valores.valorPremio,
      dataInicio: this.paraIso(valores.dataInicio),
      dataFim: this.paraIso(valores.dataFim)
    };

    const requisicao = this.modoEdicao
      ? this.apoliceService.atualizar(this.apoliceId!, payloadComum)
      : this.apoliceService.criar({
          ...payloadComum,
          documentoCliente: valores.documentoCliente,
          nomeCliente: valores.nomeCliente
        });

    requisicao.pipe(finalize(() => (this.salvando = false))).subscribe({
      next: (resposta) => {
        this.notification.sucesso(
          this.modoEdicao ? 'Apólice atualizada com sucesso.' : 'Apólice cadastrada com sucesso.'
        );
        const id = resposta.data?.id ?? this.apoliceId;
        this.router.navigate(['/apolices', id]);
      }
    });
  }

  cancelar(): void {
    if (this.modoEdicao && this.apoliceId) {
      this.router.navigate(['/apolices', this.apoliceId]);
    } else {
      this.router.navigate(['/apolices']);
    }
  }

  private paraIso(data: Date): string {
    return new Date(data).toISOString();
  }
}
