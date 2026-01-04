import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface Reservation {
  id: number;
  userId: number;
  user?: any;
  eventId: number;
  event?: any;
  seats: Seat[];
  status: string;
  createdAt: string;
  expiresAt?: string;
  totalAmount: number;
}

export interface Seat {
  id: number;
  number: string;
  row: string;
  roomId: number;
  status: number;
  zone: number; 
  price: number;
  reservationId?: number;
}

export interface CreateReservationDTO {
  userId: number;
  eventId: number;
  seatIds: number[];
}

@Injectable({
  providedIn: 'root'
})
export class ReservationService {
  private apiUrl = 'http://localhost:5000/api/Reservations';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) { }

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  getMyReservations(): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(`${this.apiUrl}/my-reservations`, { headers: this.getHeaders() });
  }

  getReservationById(id: number): Observable<Reservation> {
    return this.http.get<Reservation>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  createReservation(reservation: CreateReservationDTO): Observable<Reservation> {
    return this.http.post<Reservation>(this.apiUrl, reservation, { headers: this.getHeaders() });
  }

  confirmPayment(reservationId: number): Observable<Reservation> {
    return this.http.post<Reservation>(`${this.apiUrl}/${reservationId}/confirm-payment`, {}, { headers: this.getHeaders() });
  }

  cancelReservation(reservationId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${reservationId}/cancel`, {}, { headers: this.getHeaders() });
  }

  downloadTicketPdf(reservationId: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${reservationId}/ticket/pdf`, {
      headers: this.getHeaders(),
      responseType: 'blob' 
    });
  }

  getQRCode(reservationId: number): Observable<{ qrCode: string }> {
    return this.http.get<{ qrCode: string }>(`${this.apiUrl}/${reservationId}/ticket/qrcode`, {
      headers: this.getHeaders()
    });
  }
}
