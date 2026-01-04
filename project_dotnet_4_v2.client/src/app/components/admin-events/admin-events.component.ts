import { Component, OnInit } from '@angular/core';
import { EventService, Event, CreateEventDTO } from '../../services/event.service';
import { RoomService, Room } from '../../services/room.service';

@Component({
  selector: 'app-admin-events',
  templateUrl: './admin-events.component.html',
  styleUrls: ['./admin-events.component.css']
})
export class AdminEventsComponent implements OnInit {
  events: Event[] = [];
  rooms: Room[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  
  showForm = false;
  isEditing = false;
  currentEventId?: number;

  eventForm: CreateEventDTO = {
    name: '',
    date: new Date().toISOString(),
    description: '',
    roomId: 0,
    imageUrl: '',
    basePrice: 10,
    director: '',
    duration: 90,
    genre: ''
  };

  constructor(
    private eventService: EventService,
    private roomService: RoomService
  ) { }

  ngOnInit(): void {
    this.loadEvents();
    this.loadRooms();
  }

  loadEvents(): void {
    this.isLoading = true;
    this.eventService.getEvents().subscribe({
      next: (data) => {
        this.events = data;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Erreur lors du chargement des événements';
        this.isLoading = false;
      }
    });
  }

  loadRooms(): void {
    this.roomService.getRooms().subscribe({
      next: (data) => {
        this.rooms = data;
      },
      error: (error) => {
        console.error('Erreur lors du chargement des salles', error);
      }
    });
  }

  openCreateForm(): void {
    this.showForm = true;
    this.isEditing = false;
    this.resetForm();
  }

  openEditForm(event: Event): void {
    this.showForm = true;
    this.isEditing = true;
    this.currentEventId = event.id;
    this.eventForm = {
      name: event.name,
      date: event.date,
      description: event.description,
      roomId: event.roomId,
      imageUrl: event.imageUrl || '',
      basePrice: event.basePrice,
      director: event.director || '',
      duration: event.duration || 90,
      genre: event.genre || ''
    };
  }

  closeForm(): void {
    this.showForm = false;
    this.resetForm();
  }

  resetForm(): void {
    this.eventForm = {
      name: '',
      date: new Date().toISOString(),
      description: '',
      roomId: this.rooms.length > 0 ? this.rooms[0].id : 0,
      imageUrl: '',
      basePrice: 10,
      director: '',
      duration: 90,
      genre: ''
    };
    this.currentEventId = undefined;
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isEditing && this.currentEventId) {

      this.eventService.updateEvent(this.currentEventId, this.eventForm).subscribe({
        next: () => {
          this.successMessage = 'Success, event modified';
          this.closeForm();
          this.loadEvents();
        },
        error: (error) => {
          this.errorMessage = 'Error in the modification';
        }
      });
    } else {
      this.eventService.createEvent(this.eventForm).subscribe({
        next: () => {
          this.successMessage = 'Success, event created';
          this.closeForm();
          this.loadEvents();
        },
        error: (error) => {
          this.errorMessage = 'Error in the creation';
        }
      });
    }
  }

  deleteEvent(id: number, eventName: string): void {
    if (confirm(`Are you sure you want to delete the event ? "${eventName}" ?`)) {
      this.eventService.deleteEvent(id).subscribe({
        next: () => {
          this.successMessage = 'Success, event deleted';
          this.loadEvents();
        },
        error: (error) => {
          this.errorMessage = 'Error in the deleting';
        }
      });
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: 'long',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
