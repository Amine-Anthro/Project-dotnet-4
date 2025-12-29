import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DashboardStats {
  totalEvents: number;
  totalReservations: number;
  totalSeatsReserved: number;
  totalSeatsPaid: number;
  totalRevenue: number;
  pendingRevenue: number;
  eventsStats: EventStats[];
}

export interface EventStats {
  eventId: number;
  eventName: string;
  totalSeats: number;
  availableSeats: number;
  reservedSeats: number;
  paidSeats: number;
  fillRate: number;
  revenue: number;
  potentialRevenue: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = 'http://localhost:5000/api/Dashboard';

  constructor(private http: HttpClient) { }

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  getDashboardStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.apiUrl}/stats`, {
      headers: this.getHeaders()
    });
  }

  getEventStats(eventId: number): Observable<EventStats> {
    return this.http.get<EventStats>(`${this.apiUrl}/events/${eventId}/stats`, {
      headers: this.getHeaders()
    });
  }
}
