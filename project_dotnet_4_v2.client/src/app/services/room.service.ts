import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface Room {
  id: number;
  name: string;
  capacity: string;
  totalRows: number;
  seatsPerRow: number;
  economicPrice: number;
  vipPrice: number;
  standardPrice: number;
}

export interface CreateRoomDTO {
  name: string;
  capacity: string;
  totalRows: number;
  seatsPerRow: number;
  economicPrice: number;
  vipPrice: number;
  standardPrice: number;
}

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private apiUrl = 'http://localhost:5000/api/Room';

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

  getRooms(): Observable<Room[]> {
    return this.http.get<Room[]>(this.apiUrl);
  }

  getRoom(id: number): Observable<Room> {
    return this.http.get<Room>(`${this.apiUrl}/${id}`);
  }

  createRoom(room: CreateRoomDTO): Observable<Room> {
    return this.http.post<Room>(this.apiUrl, room, { headers: this.getHeaders() });
  }

  updateRoom(id: number, room: CreateRoomDTO): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, { id, ...room }, { headers: this.getHeaders() });
  }

  deleteRoom(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}
