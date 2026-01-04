import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EventService, Event } from '../../services/event.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class Home implements OnInit {
  events: Event[] = [];
  isLoading: boolean = true;
  errorMessage: string = '';

  constructor(
    private eventService: EventService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.eventService.getEvents().subscribe({
      next: (data) => {
        this.events = data;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error in the loading the events ', error);
        this.errorMessage = 'Impossible to load the events';
        this.isLoading = false;
      }
    });
  }

  viewEventDetails(eventId: number): void {
    this.router.navigate(['/event', eventId]);
  }
}
