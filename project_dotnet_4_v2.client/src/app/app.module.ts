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
import { adminGuard } from './guards/admin.guard';
import { AdminRoomsComponent } from './components/admin-rooms/admin-rooms.component';
import { AdminEventsComponent } from './components/admin-events/admin-events.component';

@NgModule({
  declarations: [
    AppComponent,
    Login,
    Register,
    Home,
    EventDetailComponent,
    AdminRoomsComponent,
    AdminEventsComponent
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
      { path: 'admin/rooms', component: AdminRoomsComponent, canActivate: [adminGuard] },
      { path: 'admin/events', component: AdminEventsComponent, canActivate: [adminGuard] },
      { path: '**', redirectTo: '/login' }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
