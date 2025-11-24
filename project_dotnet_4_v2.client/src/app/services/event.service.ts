import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Event {
  id: number;
  name: string;
  date: string;
  description: string;
  roomId: number;
  room?: Room;
  imageUrl?: string;
  basePrice: number;
}

export interface Room {
  id: number;
  number: string;
  capacity: string;
  seats: Seat[];
}

export interface Seat {
  id: number;
  number: string;
  row: string;
  roomId: number;
  status: 'Available' | 'Reserved' | 'Paid';
  price: number;
}

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private apiUrl = 'http://localhost:5000/api/Events';

  constructor(private http: HttpClient) { }

  
  getEvents(): Observable<Event[]> {
    return this.http.get<Event[]>(this.apiUrl);
  }

  getEvent(id: number): Observable<Event> {
    return this.http.get<Event>(`${this.apiUrl}/${id}`);
  }
}
