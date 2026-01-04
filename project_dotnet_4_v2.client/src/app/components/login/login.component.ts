import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',

  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class Login {
  username: string = '';
  password: string = '';
  errorMessage: string = '';
  isLoading: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  onLogin(): void {
    if (!this.username || !this.password) {
      this.errorMessage = 'Complete all the fields please';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.username, this.password).subscribe({
      next: (token: any) => {
        console.log('Login successfully', token);
        this.isLoading = false;
        this.router.navigate(['/home'])
      },
      error: (error: any) => {
        console.error('Error on the login', error);
        this.errorMessage = 'Invalid username or password, please retry';
        this.isLoading = false;
      }
    });
  }
}
