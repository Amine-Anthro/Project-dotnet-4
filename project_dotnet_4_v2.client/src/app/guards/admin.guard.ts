import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const token = authService.getToken();

  if (!token) {
    router.navigate(['/login']);
    return false;
  }

  // Décoder le token JWT pour vérifier le rôle
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));

    // Vérifier si le rôle est Admin
    const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    if (role === 'Admin') {
      return true;
    }
  } catch (error) {
    console.error('Erreur lors du décodage du token', error);
  }

  // Si pas admin, rediriger
  alert('Accès refusé. Vous devez être administrateur.');
  router.navigate(['/home']);
  return false;
};
