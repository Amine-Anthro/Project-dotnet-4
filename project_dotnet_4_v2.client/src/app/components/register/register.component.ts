import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, RegisterRequest } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class Register {
  username: string = '';
  password: string = '';
  confirmPassword: string = '';
  email: string = '';

  errorMessage: string = '';
  successMessage: string = '';
  isLoading: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  onRegister(): void {

    if (!this.username || !this.password || !this.email) {
      this.errorMessage = 'Rempli tous les champs stp';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = ' HAHA le nul il sait pas écrire son mot de passe de fois de suite';
      return;
    }

    if (this.password.length < 6) {
      this.errorMessage = 'Le mot de passe est aussi court que ton ... bref il doit faire plus de 6 caractères';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const userData: RegisterRequest = {
      username: this.username,
      password: this.password,
      email: this.email,
      role: 'Client'  // Rôle par défaut
    };

    this.authService.register(userData).subscribe({
      next: (response) => {
        console.log('Inscription réussie gg !', response);
        this.successMessage = 'Inscription réussie ! Redirection...';
        this.isLoading = false;

        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (error) => {
        console.error('Erreur d\'inscription', error);
        this.errorMessage = 'Erreur lors de l\'inscription. Veuillez réessayer.';
        this.isLoading = false;
      }
    });
  }
}
