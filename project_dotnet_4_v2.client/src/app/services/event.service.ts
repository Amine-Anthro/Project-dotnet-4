import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface Event {
  id: number;
  name: string;
  date: string;
  description: string;
  roomId: number;
  imageUrl?: string;
  basePrice: number;
  director?: string;
  duration?: number;
  genre?: string;
  room?: Room; 
}

export interface Room {
  id: number;
  name: string;
  capacity: string;
  totalRows: number;
  seatsPerRow: number;
  economicPrice: number;
  vipPrice: number;
  standardPrice: number;
  seats?: Seat[];
}

export interface Seat {
  id: number;
  number: string;
  row: string;
  roomId: number;
  status: string;
  zone: number; 
  price: number;
}

export interface CreateEventDTO {
  name: string;
  date: string;
  description: string;
  roomId: number;
  imageUrl?: string;
  basePrice: number;
  director?: string;
  duration?: number;
  genre?: string;
}

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private apiUrl = 'http://localhost:5000/api/Events';

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

  getEvents(): Observable<Event[]> {
    return this.http.get<Event[]>(this.apiUrl);
  }

  getEvent(id: number): Observable<Event> {
    return this.http.get<Event>(`${this.apiUrl}/${id}`);
  }

  createEvent(event: CreateEventDTO): Observable<Event> {
    return this.http.post<Event>(this.apiUrl, event, { headers: this.getHeaders() });
  }

  updateEvent(id: number, event: CreateEventDTO): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, { id, ...event }, { headers: this.getHeaders() });
  }

  deleteEvent(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}
