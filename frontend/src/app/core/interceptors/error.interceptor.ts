import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (ehConsultaDeClienteNaoEncontrado(req.url, error.status)) {
        return throwError(() => error);
      }

      const mensagem = extrairMensagem(error);
      snackBar.open(mensagem, 'Fechar', { duration: 5000 });
      return throwError(() => error);
    })
  );
};

function ehConsultaDeClienteNaoEncontrado(url: string, status: number): boolean {
  return status === 404 && /\/clientes\//.test(url);
}

function extrairMensagem(error: HttpErrorResponse): string {
  if (error.error?.errors?.length) {
    return error.error.errors.join(' ');
  }

  if (error.error?.message) {
    return error.error.message;
  }

  if (error.status === 0) {
    return 'Não foi possível conectar à API.';
  }

  return 'Ocorreu um erro inesperado.';
}
