import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';


@Component({
  selector: 'app-root',  
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  username: string | null = null;
  role: string | null = null;

  constructor(
    public authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.updateUserInfo();

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.updateUserInfo();
    });
  
  }

  updateUserInfo(): void {
    if (this.authService.isLoggedIn()) {
      const user = this.authService.getCurrentUser();
      this.username = user?.username || null;
      this.role = user?.role || null;
    }
  }

  logout(): void {
    this.authService.logout();
    this.username = null;
    this.role = null;
    this.router.navigate(['/login']);
  }
}
