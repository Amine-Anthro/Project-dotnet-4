import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ReservationService, Reservation } from '../../services/reservation.service';

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css']
})
export class PaymentComponent implements OnInit {
  reservation?: Reservation;
  isLoading = true;
  isProcessing = false;
  errorMessage = '';
  successMessage = '';

  cardNumber: string = '';
  cardName: string = '';
  cardExpiry: string = '';
  cardCVV: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private reservationService: ReservationService
  ) { }

  ngOnInit(): void {
    const reservationId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadReservation(reservationId);
  }

  private getStatusValue(status: string | number): number {
    if (typeof status === 'number') {
      return status;
    }

    switch (status) {
      case 'InProgress': return 0;
      case 'Paid': return 1;
      case 'Cancelled': return 2;
      default: return -1;
    }
  }

  loadReservation(id: number): void {
    this.reservationService.getReservationById(id).subscribe({
      next: (data) => {
        this.reservation = data;

        const statusValue = this.getStatusValue(data.status);

        if (statusValue === 1) {
          this.errorMessage = 'This reservation has already been paid';
          setTimeout(() => {
            this.router.navigate(['/my-reservations']);
          }, 2000);
        }

        if (statusValue === 2 || data.status === 'Cancelled') {
          this.errorMessage = 'This reservation has been cancelled';
          setTimeout(() => {
            this.router.navigate(['/home']);
          }, 2000);
        }

        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading reservation', error);
        this.errorMessage = 'Unable to load reservation';
        this.isLoading = false;
      }
    });
  }

  processPayment(): void {
    if (!this.validatePayment()) {
      return;
    }

    this.isProcessing = true;
    this.errorMessage = '';

    setTimeout(() => {
      this.reservationService.confirmPayment(this.reservation!.id).subscribe({
        next: (updatedReservation) => {
          this.successMessage = 'Payment confirmed! Redirecting...';
          this.isProcessing = false;

          setTimeout(() => {
            this.router.navigate(['/my-reservations']);
          }, 2000);
        },
        error: (error) => {
          console.error('Error processing payment', error);
          this.errorMessage = error.error?.message || 'Error processing payment';
          this.isProcessing = false;
        }
      });
    }, 1500);
  }

  validatePayment(): boolean {
    if (!this.cardNumber || this.cardNumber.length !== 16) {
      this.errorMessage = 'Invalid card number (16 digits required)';
      return false;
    }
    if (!this.cardName) {
      this.errorMessage = 'Cardholder name required';
      return false;
    }
    if (!this.cardExpiry || !/^\d{2}\/\d{2}$/.test(this.cardExpiry)) {
      this.errorMessage = 'Invalid expiration date (MM/YY)';
      return false;
    }
    if (!this.cardCVV || this.cardCVV.length !== 3) {
      this.errorMessage = 'Invalid CVV (3 digits)';
      return false;
    }

    return true;
  }

  cancelReservation(): void {
    if (!confirm('Are you sure you want to cancel this reservation?')) {
      return;
    }

    this.reservationService.cancelReservation(this.reservation!.id).subscribe({
      next: () => {
        alert('Reservation cancelled successfully');
        this.router.navigate(['/home']);
      },
      error: (error) => {
        console.error('Error cancelling reservation', error);
        this.errorMessage = 'Error cancelling reservation';
      }
    });
  }

  formatExpiresAt(expiresAt?: string): string {
    if (!expiresAt) return '';

    const expireDate = new Date(expiresAt);
    const now = new Date();
    const diff = expireDate.getTime() - now.getTime();
    const minutes = Math.floor(diff / 60000);
    const seconds = Math.floor((diff % 60000) / 1000);

    if (minutes < 0) return 'Expired';
    return `${minutes}m ${seconds}s`;
  }

  getStatusBadgeClass(status: string | number): string {
    const statusValue = this.getStatusValue(status);

    switch (statusValue) {
      case 0: return 'status-pending';
      case 1: return 'status-paid';
      case 2: return 'status-cancelled';
      default: return '';
    }
  }

  getStatusText(status: string | number): string {
    const statusValue = this.getStatusValue(status);

    switch (statusValue) {
      case 0: return 'Pending payment';
      case 1: return 'Paid';
      case 2: return 'Cancelled';
      default: return String(status);
    }
  }

  isInProgress(): boolean {
    if (!this.reservation) return false;
    return this.getStatusValue(this.reservation.status) === 0;
  }

  getZoneName(zone: number): string {
    switch (zone) {
      case 0: return 'Economic';
      case 1: return 'VIP';
      case 2: return 'Standard';
      default: return '';
    }
  }
}
