import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService, Event, Seat } from '../../services/event.service';

@Component({
  selector: 'app-event-detail',
  templateUrl: './event-detail.component.html',
  styleUrls: ['./event-detail.component.css']
})
export class EventDetailComponent implements OnInit {
  event?: Event;
  selectedSeats: Seat[] = [];
  isLoading: boolean = true;
  errorMessage: string = '';
  totalPrice: number = 0;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService
  ) { }

  ngOnInit(): void {
    const eventId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadEventDetails(eventId);
    
  }

  loadEventDetails(id: number): void {
    this.eventService.getEvent(id).subscribe({
      next: (data) => {
        this.event = data;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Erreur lors du chargement de l\'événement', error);
        this.errorMessage = 'Impossible de charger les détails de l\'événement';
        this.isLoading = false;
      }
    });
  }

  getSeatsByRow(): { [key: string]: Seat[] } {
    if (!this.event?.room?.seats) return {};

    const seatsByRow: { [key: string]: Seat[] } = {};

    this.event.room.seats.forEach((seat: Seat) => {
      if (!seatsByRow[seat.row]) {
        seatsByRow[seat.row] = [];
      }
      seatsByRow[seat.row].push(seat);
    });

    
    Object.keys(seatsByRow).forEach(row => {
      seatsByRow[row].sort((a, b) => Number(a.number) - Number(b.number));
    });

    return seatsByRow;
  }

  getRows(): string[] {
    return Object.keys(this.getSeatsByRow()).sort();
  }

  getSeatClass(seat: Seat): string {
    if (this.isSeatSelected(seat)) {
      return 'seat selected';
    }

    switch (seat.status) {
      case 'Available':
        return 'seat available';
      case 'Reserved':
        return 'seat reserved';
      case 'Paid':
        return 'seat paid';
      default:
        return 'seat';
    }
  }

  isSeatSelected(seat: Seat): boolean {
    return this.selectedSeats.some(s => s.id === seat.id);
  }

  toggleSeat(seat: Seat): void {
    if (seat.status !== 'Available') {
      return; 
    }

    if (this.isSeatSelected(seat)) {
      
      this.selectedSeats = this.selectedSeats.filter(s => s.id !== seat.id);
    } else {
      
      this.selectedSeats.push(seat);
    }

    this.calculateTotal();
  }

  calculateTotal(): void {
    this.totalPrice = this.selectedSeats.reduce((sum, seat) => sum + seat.price, 0);
  }

  proceedToReservation(): void {
    if (this.selectedSeats.length === 0) {
      alert('Veuillez sélectionner au moins un siège');
      return;
    }

    
    console.log('Réservation de', this.selectedSeats.length, 'siège(s)');
    console.log('Prix total:', this.totalPrice, '€');

   
    alert(`Réservation de ${this.selectedSeats.length} siège(s) pour ${this.totalPrice.toFixed(2)}€`);
  }

  goBack(): void {
    this.router.navigate(['/home']);
  }
}
