import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ReservationService, Reservation } from '../../services/reservation.service';

@Component({
  selector: 'app-my-reservations',
  templateUrl: './my-reservations.component.html',
  styleUrls: ['./my-reservations.component.css']
})
export class MyReservationsComponent implements OnInit {
  reservations: Reservation[] = [];
  isLoading = true;
  errorMessage = '';
  successMessage = '';

  constructor(
    private reservationService: ReservationService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadReservations();
  }

  loadReservations(): void {
    this.reservationService.getMyReservations().subscribe({
      next: (data) => {
        this.reservations = data.sort((a, b) => {
          return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
        });
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Erreur lors du chargement des réservations', error);
        this.errorMessage = 'Impossible de charger vos réservations';
        this.isLoading = false;
      }
    });
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
      case 0: return 'En attente';
      case 1: return 'Payée';
      case 2: return 'Annulée';
      default: return String(status);
    }
  }

  getZoneName(zone: number): string {
    switch (zone) {
      case 0: return 'Économique';
      case 1: return 'VIP';
      case 2: return 'Standard';
      default: return '';
    }
  }

  isInProgress(reservation: Reservation): boolean {
    return this.getStatusValue(reservation.status) === 0;
  }

  isPaid(reservation: Reservation): boolean {
    return this.getStatusValue(reservation.status) === 1;
  }

  goToPayment(reservationId: number): void {
    this.router.navigate(['/payment', reservationId]);
  }

  cancelReservation(reservation: Reservation): void {
    if (reservation.status === 'Paid') {
      alert('Impossible d\'annuler une réservation déjà payée');
      return;
    }

    if (!confirm(`Êtes-vous sûr de vouloir annuler la réservation #${reservation.id} ?`)) {
      return;
    }

    this.reservationService.cancelReservation(reservation.id).subscribe({
      next: () => {
        this.successMessage = 'Réservation annulée avec succès';
        this.loadReservations();

        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        console.error('Erreur lors de l\'annulation', error);
        this.errorMessage = error.error?.message || 'Erreur lors de l\'annulation';
      }
    });
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
