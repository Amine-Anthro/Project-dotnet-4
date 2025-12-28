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

  // Formulaire de paiement fictif
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
    // Convertir string en nombre
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

        // Vérifier si déjà payée
        if (statusValue === 1) {
          this.errorMessage = 'Cette réservation a déjà été payée';
          setTimeout(() => {
            this.router.navigate(['/my-reservations']);
          }, 2000);
        }

        // Vérifier si annulée
        if (statusValue === 2 || data.status === 'Cancelled') {
          this.errorMessage = 'Cette réservation a été annulée';
          setTimeout(() => {
            this.router.navigate(['/home']);
          }, 2000);
        }

        this.isLoading = false;
      },
      error: (error) => {
        console.error('Erreur lors du chargement de la réservation', error);
        this.errorMessage = 'Impossible de charger la réservation';
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

    // Simulation d'un délai de traitement (1.5 secondes)
    setTimeout(() => {
      this.reservationService.confirmPayment(this.reservation!.id).subscribe({
        next: (updatedReservation) => {
          this.successMessage = 'Paiement confirmé ! Redirection...';
          this.isProcessing = false;

          // Redirection vers "Mes réservations" après 2 secondes
          setTimeout(() => {
            this.router.navigate(['/my-reservations']);
          }, 2000);
        },
        error: (error) => {
          console.error('Erreur lors du paiement', error);
          this.errorMessage = error.error?.message || 'Erreur lors du paiement';
          this.isProcessing = false;
        }
      });
    }, 1500);
  }

  validatePayment(): boolean {
   
      if (!this.cardNumber || this.cardNumber.length !== 16) {
        this.errorMessage = 'Numéro de carte invalide (16 chiffres requis)';
        return false;
      }
      if (!this.cardName) {
        this.errorMessage = 'Nom du titulaire requis';
        return false;
      }
      if (!this.cardExpiry || !/^\d{2}\/\d{2}$/.test(this.cardExpiry)) {
        this.errorMessage = 'Date d\'expiration invalide (MM/YY)';
        return false;
      }
      if (!this.cardCVV || this.cardCVV.length !== 3) {
        this.errorMessage = 'CVV invalide (3 chiffres)';
        return false;
      }
    
    return true;
  }

  cancelReservation(): void {
    if (!confirm('Êtes-vous sûr de vouloir annuler cette réservation ?')) {
      return;
    }

    this.reservationService.cancelReservation(this.reservation!.id).subscribe({
      next: () => {
        alert('Réservation annulée avec succès');
        this.router.navigate(['/home']);
      },
      error: (error) => {
        console.error('Erreur lors de l\'annulation', error);
        this.errorMessage = 'Erreur lors de l\'annulation';
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

    if (minutes < 0) return 'Expiré';
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
      case 0: return 'En attente de paiement';
      case 1: return 'Payée';
      case 2: return 'Annulée';
      default: return String(status);
    }
  }

  isInProgress(): boolean {
    if (!this.reservation) return false;
    return this.getStatusValue(this.reservation.status) === 0;
  }

  getZoneName(zone: number): string {
    switch (zone) {
      case 0: return 'Économique';
      case 1: return 'VIP';
      case 2: return 'Standard';
      default: return '';
    }
  }
}
