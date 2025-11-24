import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { Login } from './components/login/login.component';
import { Register } from './components/register/register.component';
import { Home } from './components/home/home.component';
import { RouterModule } from '@angular/router';
import { EventDetailComponent } from './components/event-detail/event-detail.component';

@NgModule({
  declarations: [
    AppComponent,
    Login,
    Register,
    Home,
    EventDetailComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', redirectTo: '/login', pathMatch: 'full' },
      { path: 'login', component: Login },
      { path: 'register', component: Register },
      { path: 'home', component: Home },
      { path: 'event/:id', component: EventDetailComponent },
      { path: '**', redirectTo: '/login' }

    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
