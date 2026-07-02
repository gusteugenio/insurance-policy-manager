import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'apolices', pathMatch: 'full' },
  {
    path: 'apolices',
    loadComponent: () =>
      import('./features/apolices/apolice-listagem/apolice-listagem.component').then(m => m.ApoliceListagemComponent)
  },
  {
    path: 'apolices/novo',
    loadComponent: () =>
      import('./features/apolices/apolice-form/apolice-form.component').then(m => m.ApoliceFormComponent)
  },
  {
    path: 'apolices/:id/editar',
    loadComponent: () =>
      import('./features/apolices/apolice-form/apolice-form.component').then(m => m.ApoliceFormComponent)
  },
  {
    path: 'apolices/:id',
    loadComponent: () =>
      import('./features/apolices/apolice-detalhe/apolice-detalhe.component').then(m => m.ApoliceDetalheComponent)
  },
  {
    path: 'clientes',
    loadComponent: () =>
      import('./features/clientes/cliente-listagem/cliente-listagem.component').then(m => m.ClienteListagemComponent)
  }
];
