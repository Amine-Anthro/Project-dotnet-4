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
      await this.signalrService.startConnection();
      await this.signalrService.joinEventRoom(eventId);

      this.seatReservedSub = this.signalrService.seatReserved$.subscribe(
        (update: SeatUpdate) => this.handleSeatReserved(update)
      );

      this.seatReleasedSub = this.signalrService.seatReleased$.subscribe(
        (update: SeatUpdate) => this.handleSeatReleased(update)
      );

      this.seatPaidSub = this.signalrService.seatPaid$.subscribe(
        (update: SeatUpdate) => this.handleSeatPaid(update)
      );

      console.log('SignalR connected and listening');
    } catch (error) {
      console.error('SignalR connection error:', error);
    }
  }

  handleSeatReserved(update: SeatUpdate): void {
    console.log('Seat reserved:', update);
    this.updateSeatStatus(update.seatId, 1); 
  }

  handleSeatReleased(update: SeatUpdate): void {
    console.log('Seat released:', update);
    this.updateSeatStatus(update.seatId, 0); 
  }

  handleSeatPaid(update: SeatUpdate): void {
    console.log('Seat paid:', update);
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
          this.errorMessage = `Seat ${seat.row}${seat.number} has just been reserved by another user`;
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
        console.error('Error loading event', error);
        this.errorMessage = 'Unable to load event details';
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
    if (seat.status !== 0) { 
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
      alert('Please select at least one seat');
      return;
    }
   
    const token = this.authService.getToken();
    if (!token) {
      this.errorMessage = 'Session expired';
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
        this.successMessage = `Reservation created successfully! (ID: ${reservation.id})`;
        this.isProcessing = false;

        setTimeout(() => {
          this.router.navigate(['/payment', reservation.id]);
        }, 2000);
      },
      error: (error) => {
        console.error('Error creating reservation', error);
        this.errorMessage = error.error?.message || 'Error creating reservation';
        this.isProcessing = false;
        this.loadEventDetails(this.event!.id);
      }
    });
    
    console.log('Reserving', this.selectedSeats.length, 'seat(s)');
    console.log('Total price:', this.totalPrice, '$');

    alert(`Reservation of ${this.selectedSeats.length} seat(s) for $${this.totalPrice.toFixed(2)}`);
  }

  goBack(): void {
    this.router.navigate(['/home']);
  }
}
