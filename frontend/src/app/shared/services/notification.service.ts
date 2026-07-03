import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  constructor(private snackBar: MatSnackBar) {}

  sucesso(mensagem: string): void {
    this.snackBar.open(mensagem, 'Fechar', {
      duration: 4000,
      panelClass: 'snackbar-sucesso'
    });
  }

  info(mensagem: string): void {
    this.snackBar.open(mensagem, 'Fechar', { duration: 4000 });
  }
}
