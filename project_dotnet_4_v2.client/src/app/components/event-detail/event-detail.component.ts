import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService, Event, Seat } from '../../services/event.service';
import { AuthService } from '../../services/auth.service';
import { ReservationService, CreateReservationDTO } from '../../services/reservation.service';
import { SignalrService, SeatUpdate } from '../../services/signalr.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-event-detail',
  templateUrl: './event-detail.component.html',
  styleUrls: ['./event-detail.component.css']
})
export class EventDetailComponent implements OnInit, OnDestroy {
  event?: Event;
  selectedSeats: Seat[] = [];
  isLoading: boolean = true;
  errorMessage: string = '';
  successMessage: string = '';
  totalPrice: number = 0;
  isProcessing: boolean = false;

  private seatReservedSub?: Subscription;
  private seatReleasedSub?: Subscription;
  private seatPaidSub?: Subscription;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    private reservationService: ReservationService,
    private authService: AuthService,
    private signalrService: SignalrService
  ) { }

  ngOnInit(): void {
    const eventId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadEventDetails(eventId);
    this.connectToSignalR(eventId);
  }

  ngOnDestroy(): void {
    
    this.seatReservedSub?.unsubscribe();
    this.seatReleasedSub?.unsubscribe();
    this.seatPaidSub?.unsubscribe();

  
    if (this.event) {
      this.signalrService.leaveEventRoom(this.event.id);
    }
  }

  async connectToSignalR(eventId: number): Promise<void> {
    try {
      // Démarrer la connexion (si pas déjà fait)
      await this.signalrService.startConnection();

      // Rejoindre la room de l'événement
      await this.signalrService.joinEventRoom(eventId);

      // Écouter les mises à jour de sièges
      this.seatReservedSub = this.signalrService.seatReserved$.subscribe(
        (update: SeatUpdate) => this.handleSeatReserved(update)
      );

      this.seatReleasedSub = this.signalrService.seatReleased$.subscribe(
        (update: SeatUpdate) => this.handleSeatReleased(update)
      );

      this.seatPaidSub = this.signalrService.seatPaid$.subscribe(
        (update: SeatUpdate) => this.handleSeatPaid(update)
      );

      console.log(' SignalR connected and listening');
    } catch (error) {
      console.error(' SignalR connection error:', error);
    }
  }

  handleSeatReserved(update: SeatUpdate): void {
    console.log(' Seat reserved:', update);
    this.updateSeatStatus(update.seatId, 1); 
  }

  handleSeatReleased(update: SeatUpdate): void {
    console.log(' Seat released:', update);
    this.updateSeatStatus(update.seatId, 0); 
  }

  handleSeatPaid(update: SeatUpdate): void {
    console.log(' Seat paid:', update);
    this.updateSeatStatus(update.seatId, 2); 
  }

  updateSeatStatus(seatId: number, newStatus: number): void {
    if (!this.event?.room?.seats) return;

    const seat = this.event.room.seats.find(s => s.id === seatId);
    if (seat) {
      seat.status = newStatus;

      
      if (newStatus !== 0) {
        const index = this.selectedSeats.findIndex(s => s.id === seatId);
        if (index !== -1) {
          this.selectedSeats.splice(index, 1);
          this.calculateTotal();
          this.errorMessage = `Le siège ${seat.row}${seat.number} vient d'être réservé par un autre utilisateur`;
          setTimeout(() => this.errorMessage = '', 3000);
        }
      }
    }
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
        this.loadEventDetails(this.event!.id);
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
