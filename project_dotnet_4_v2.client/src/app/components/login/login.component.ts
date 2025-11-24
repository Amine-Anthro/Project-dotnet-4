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
      this.errorMessage = 'Faut remplir tout les champs frérot ou soeurette';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.username, this.password).subscribe({
      next: (token: any) => {
        console.log('Le ou la gigachad est bien login', token);
        this.isLoading = false;
        this.router.navigate(['/home'])
      },
      error: (error: any) => {
        console.error('Nan le login a eu une embrouille (-1000 aura)', error);
        this.errorMessage = 'Askip tes indentifiants sont invalides, tu pues sa mère';
        this.isLoading = false;
      }
    });
  }
}
