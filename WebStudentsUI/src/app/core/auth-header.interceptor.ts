import { HttpInterceptorFn } from '@angular/common/http';

export const authHeaderInterceptor: HttpInterceptorFn = (req, next) => {
  const rawUser = localStorage.getItem('user');
  if (!rawUser) {
    return next(req);
  }

  try {
    const user = JSON.parse(rawUser);
    const role = user?.role as string | undefined;
    const linkedPersonId = user?.linkedPersonId as string | undefined;

    let headers = req.headers;

    if (role) {
      headers = headers.set('X-Role', role);
    }

    if (linkedPersonId) {
      headers = headers.set('X-LinkedPersonId', linkedPersonId);
    }

    return next(req.clone({ headers }));
  } catch {
    return next(req);
  }
};
