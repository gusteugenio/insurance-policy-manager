import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export const baseUrlInterceptor: HttpInterceptorFn = (req, next) => {
  if (/^https?:\/\//i.test(req.url)) {
    return next(req);
  }

  const url = `${environment.apiUrl}${req.url.startsWith('/') ? '' : '/'}${req.url}`;
  return next(req.clone({ url }));
};
