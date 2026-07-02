import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'apolices', pathMatch: 'full' },
  {
    path: 'apolices',
    loadComponent: () =>
      import('./features/apolices/apolice-listagem/apolice-listagem.component').then(m => m.ApoliceListagemComponent)
  },
  {
    path: 'clientes',
    loadComponent: () =>
      import('./features/clientes/cliente-listagem/cliente-listagem.component').then(m => m.ClienteListagemComponent)
  }
];
