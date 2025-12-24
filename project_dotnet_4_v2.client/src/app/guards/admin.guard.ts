import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  let token = authService.getToken();

  if (!token) {
    router.navigate(['/login']);
    return false;
  }

  token = token.trim().replace(/^["']|["']$/g, '').replace('Bearer ', '');

  console.log('Token nettoyé:', token);
  // Décode le token JWT pour vérifier le rôle
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));

    console.log('Token payload complet:', payload);
    console.log('Toutes les clés:', Object.keys(payload));

    // Vérifier si le rôle est Admin
    const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      || payload['role']
      || payload['Role']

    console.log('Rôle trouvé:', role);

    if (role === 'Admin') {
      return true;
    }
  } catch (error) {
    console.error('Error decoding the token', error);
  }

  
  alert('Access denied. You must be an administrator.');
  router.navigate(['/home']);
  return false;
};
