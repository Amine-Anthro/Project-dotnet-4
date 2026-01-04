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
      this.errorMessage = 'Please fill in all fields';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match';
      return;
    }

    if (this.password.length < 6) {
      this.errorMessage = 'Password must be at least 6 characters long';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const userData: RegisterRequest = {
      username: this.username,
      password: this.password,
      email: this.email,
      role: 'Client'
    };

    this.authService.register(userData).subscribe({
      next: (response) => {
        console.log('Registration successful!', response);
        this.successMessage = 'Registration successful! Redirecting...';
        this.isLoading = false;

        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (error) => {
        console.error('Registration error', error);
        this.errorMessage = 'Error during registration. Please try again.';
        this.isLoading = false;
      }
    });
  }
}
