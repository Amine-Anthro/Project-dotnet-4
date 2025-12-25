import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService, Event, Seat } from '../../services/event.service';
import { AuthService } from '../../services/auth.service';
import { ReservationService, CreateReservationDTO } from '../../services/reservation.service';

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
  successMessage: string = '';
  totalPrice: number = 0;
  isProcessing: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    private reservationService: ReservationService,
    private authService: AuthService
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
      case 0:
        return `seat available zone-${seat.zone}`;
      case 1:
        return 'seat reserved';
      case 2:
        return 'seat paid';
      default:
        return 'seat';
    }
  }

  getZoneName(zone: number): string {
    switch (zone) {
      case 0: return 'Economic';
      case 1: return 'VIP';
      case 2: return 'Standard';
      default: return '';
    }
  }
  isSeatSelected(seat: Seat): boolean {
    return this.selectedSeats.some(s => s.id === seat.id);
  }

  toggleSeat(seat: Seat): void {
    if (seat.status !== 0) { // Unavaiable
      return; 
    }
    // en gros si ta place-là, elle est déjà sélectionné (pendant que tu choisis où tu vas poser ton cul)
    if (this.isSeatSelected(seat)) {
      // on la dégage de la liste des places déjà select
      this.selectedSeats = this.selectedSeats.filter(s => s.id !== seat.id);
    } else {
      // sinon on la met dans la liste 
      this.selectedSeats.push(seat);
    }

    this.calculateTotal();
  }

  // il compte en accumulant les prix des sièges, comme un cpt 
  calculateTotal(): void {
    this.totalPrice = this.selectedSeats.reduce((sum, seat) => sum + seat.price, 0);
  }

  proceedToReservation(): void {
    if (this.selectedSeats.length === 0) {
      alert('Veuillez sélectionner au moins un siège');
      return;
    }
    // Récupérer l'userId depuis le token
    const token = this.authService.getToken();
    if (!token) {
      this.errorMessage = 'Session expirée';
      this.isProcessing = false;
      return;
    }

    const payload = JSON.parse(atob(token.split('.')[1]));
    const userId = parseInt(payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']);

    const reservationDto: CreateReservationDTO = {
      userId: userId,
      eventId: this.event!.id,
      seatIds: this.selectedSeats.map(s => s.id)
    };

    this.reservationService.createReservation(reservationDto).subscribe({
      next: (reservation) => {
        this.successMessage = `Réservation créée avec succès ! (ID: ${reservation.id})`;
        this.isProcessing = false;

        // Rediriger vers la page de paiement après 2 secondes
        setTimeout(() => {
          this.router.navigate(['/payment', reservation.id]);
        }, 2000);
      },
      error: (error) => {
        console.error('Erreur lors de la réservation', error);
        this.errorMessage = error.error?.message || 'Erreur lors de la réservation';
        this.isProcessing = false;
      }
    });
    
    console.log('Réservation de', this.selectedSeats.length, 'siège(s)');
    console.log('Prix total:', this.totalPrice, '€');

   
    alert(`Réservation de ${this.selectedSeats.length} siège(s) pour ${this.totalPrice.toFixed(2)}€`);
  }

  goBack(): void {
    this.router.navigate(['/home']);
  }
}
