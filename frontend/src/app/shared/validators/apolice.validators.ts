import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

const PLACA_REGEX = /^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$/;

export function placaValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const valor = (control.value ?? '').toString().trim().toUpperCase().replace(/-/g, '');
    if (!valor) return null;
    return PLACA_REGEX.test(valor) ? null : { placaInvalida: true };
  };
}

export function documentoValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const raw = (control.value ?? '').toString();
    if (!raw.trim()) return null;

    const documento = raw.replace(/\D/g, '');

    if (documento.length === 11) {
      return cpfValido(documento) ? null : { documentoInvalido: true };
    }
    if (documento.length === 14) {
      return cnpjValido(documento) ? null : { documentoInvalido: true };
    }
    return { documentoInvalido: true };
  };
}

function cpfValido(cpf: string): boolean {
  if (new Set(cpf.split('')).size === 1) return false;

  const mult1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
  const mult2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

  let tempCpf = cpf.substring(0, 9);
  let soma = 0;
  for (let i = 0; i < 9; i++) soma += Number(tempCpf[i]) * mult1[i];

  let resto = soma % 11;
  let digito1 = resto < 2 ? 0 : 11 - resto;

  tempCpf += digito1;
  soma = 0;
  for (let i = 0; i < 10; i++) soma += Number(tempCpf[i]) * mult2[i];

  resto = soma % 11;
  const digito2 = resto < 2 ? 0 : 11 - resto;

  return cpf.endsWith(`${digito1}${digito2}`);
}

function cnpjValido(cnpj: string): boolean {
  if (new Set(cnpj.split('')).size === 1) return false;

  const mult1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
  const mult2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

  let tempCnpj = cnpj.substring(0, 12);
  let soma = 0;
  for (let i = 0; i < 12; i++) soma += Number(tempCnpj[i]) * mult1[i];

  let resto = soma % 11;
  const digito1 = resto < 2 ? 0 : 11 - resto;

  tempCnpj += digito1;
  soma = 0;
  for (let i = 0; i < 13; i++) soma += Number(tempCnpj[i]) * mult2[i];

  resto = soma % 11;
  const digito2 = resto < 2 ? 0 : 11 - resto;

  return cnpj.endsWith(`${digito1}${digito2}`);
}

export function valorPremioValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const valor = control.value;
    if (valor === null || valor === undefined || valor === '') return null;
    return Number(valor) > 0 ? null : { valorPremioInvalido: true };
  };
}

export function dataInicioAntesDataFimValidator(): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const dataInicio = group.get('dataInicio')?.value;
    const dataFim = group.get('dataFim')?.value;

    if (!dataInicio || !dataFim) return null;

    const inicio = new Date(dataInicio).getTime();
    const fim = new Date(dataFim).getTime();

    return inicio < fim ? null : { dataInicioAposDataFim: true };
  };
}

export function formatarDocumento(valor: string): string {
  const digitos = (valor || '').replace(/\D/g, '').slice(0, 14);

  if (digitos.length <= 11) {
    return digitos
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
  }

  return digitos
    .replace(/(\d{2})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d)/, '$1/$2')
    .replace(/(\d{4})(\d{1,2})$/, '$1-$2');
}
