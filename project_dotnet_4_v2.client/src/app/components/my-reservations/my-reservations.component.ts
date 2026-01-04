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

  downloadingTickets: Set<number> = new Set(); 

  downloadTicket(reservationId: number): void {
    this.downloadingTickets.add(reservationId);

    this.reservationService.downloadTicketPdf(reservationId).subscribe({
      next: (blob) => {
        
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Ticket-Reservation-${reservationId}.pdf`;
        link.click();

       
        window.URL.revokeObjectURL(url);
        this.downloadingTickets.delete(reservationId);

        this.successMessage = 'Billet téléchargé avec succès !';
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        console.error('Erreur lors du téléchargement', error);
        this.errorMessage = 'Erreur lors du téléchargement du billet';
        this.downloadingTickets.delete(reservationId);
      }
    });
  }

  viewQRCode(reservationId: number): void {
    this.reservationService.getQRCode(reservationId).subscribe({
      next: (response) => {
       
        this.showQRCodeModal(response.qrCode, reservationId);
      },
      error: (error) => {
        console.error('Erreur lors de la récupération du QR code', error);
        this.errorMessage = 'Erreur lors de la récupération du QR code';
      }
    });
  }

  showQRCodeModal(qrCodeBase64: string, reservationId: number): void {
    
    const modal = document.createElement('div');
    modal.style.cssText = `
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: rgba(0,0,0,0.8);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 10000;
    `;

    modal.innerHTML = `
      <div style="background: white; padding: 2rem; border-radius: 12px; text-align: center; max-width: 400px;">
        <h3 style="margin: 0 0 1rem 0;">QR Code - Réservation #${reservationId}</h3>
        <img src="data:image/png;base64,${qrCodeBase64}" style="width: 250px; height: 250px;" />
        <p style="margin: 1rem 0; color: #666;">Présentez ce QR code à l'entrée</p>
        <button id="closeModal" style="
          padding: 0.75rem 2rem;
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          color: white;
          border: none;
          border-radius: 8px;
          font-weight: 600;
          cursor: pointer;
        ">Fermer</button>
      </div>
    `;

    document.body.appendChild(modal);

   
    modal.addEventListener('click', (e) => {
      if (e.target === modal || (e.target as HTMLElement).id === 'closeModal') {
        document.body.removeChild(modal);
      }
    });
  }

  isDownloading(reservationId: number): boolean {
    return this.downloadingTickets.has(reservationId);
  }
}
