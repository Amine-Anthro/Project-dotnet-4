import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';

export interface SeatUpdate {
  seatId: number;
  row: string;
  number: string;
  status: string;
}

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection?: signalR.HubConnection;

  // Observables pour les mises à jour
  public seatReserved$ = new Subject<SeatUpdate>();
  public seatReleased$ = new Subject<SeatUpdate>();
  public seatPaid$ = new Subject<SeatUpdate>();

  constructor() { }

  public startConnection(): Promise<void> {

    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      console.log(' Already connected to SignalR');
      return Promise.resolve();
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:5000/hubs/event')
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Debug)
      .build();

    return this.hubConnection
      .start()
      .then(() => {
        console.log(' SignalR connected');
        console.log(' Transport:', this.hubConnection?.baseUrl);
        this.registerHandlers();
      })
      .catch(err => {
        console.error(' Error connecting to SignalR:', err);
        throw err;
      });
  }

  private registerHandlers(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('SeatReserved', (data: SeatUpdate) => {
      console.log(' Seat reserved:', data);
      this.seatReserved$.next(data);
    });

    this.hubConnection.on('SeatReleased', (data: SeatUpdate) => {
      console.log(' Seat released:', data);
      this.seatReleased$.next(data);
    });

    this.hubConnection.on('SeatPaid', (data: SeatUpdate) => {
      console.log(' Seat paid:', data);
      this.seatPaid$.next(data);
    });
  }

  public joinEventRoom(eventId: number): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('No connection');
    }

    return this.hubConnection.invoke('JoinEventRoom', eventId)
      .then(() => console.log(` Joined event room: ${eventId}`))
      .catch(err => console.error('Error joining room:', err));
  }

  public leaveEventRoom(eventId: number): Promise<void> {
    if (!this.hubConnection || this.hubConnection.state !== signalR.HubConnectionState.Connected) {
      return Promise.reject('No connection');
      return Promise.resolve();
    }

    return this.hubConnection.invoke('LeaveEventRoom', eventId)
      .then(() => console.log(` Left event room: ${eventId}`))
      .catch(err => console.error('Error leaving room:', err));
  }

  public stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop()
        .then(() => console.log('SignalR disconnected'))
        .catch(err => console.error('Error disconnecting:', err));
    }
  }
}
