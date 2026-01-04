import { Injectable, NgZone } from '@angular/core';
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

  
  public seatReserved$ = new Subject<SeatUpdate>();
  public seatReleased$ = new Subject<SeatUpdate>();
  public seatPaid$ = new Subject<SeatUpdate>();
  public dashboardUpdated$ = new Subject<any>();
  public eventStatsUpdated$ = new Subject<any>();
  constructor(private ngZone: NgZone) { } 

  public startConnection(): Promise<void> {

    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      console.log(' Already connected to SignalR');
      return Promise.resolve();
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/hubs/event')
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
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
      this.ngZone.run(() => { 
        console.log('Seat reserved:', data);
        this.seatReserved$.next(data);
      });
    });

    this.hubConnection.on('SeatReleased', (data: SeatUpdate) => {
      console.log(' Seat released:', data);
      this.seatReleased$.next(data);
    });

    this.hubConnection.on('SeatPaid', (data: SeatUpdate) => {
      console.log(' Seat paid:', data);
      this.seatPaid$.next(data);
    });

    this.hubConnection.on('DashboardUpdated', (data: any) => {
      console.log(' Dashboard updated:', data);
      this.dashboardUpdated$.next(data);
    });

    this.hubConnection.on('EventStatsUpdated', (data: any) => {
      console.log(' Event stats updated:', data);
      this.eventStatsUpdated$.next(data);
    });
  }

  public joinDashboard(): Promise<void> {
    if (!this.hubConnection || this.hubConnection.state !== signalR.HubConnectionState.Connected) {
      console.warn(' Cannot join dashboard: not connected');
      return Promise.reject('Not connected');
    }

    return this.hubConnection.invoke('JoinDashboard')
      .then(() => console.log(' Joined dashboard'))
      .catch(err => console.error(' Error joining dashboard:', err));
  }

  public leaveDashboard(): Promise<void> {
    if (!this.hubConnection || this.hubConnection.state !== signalR.HubConnectionState.Connected) {
      console.warn(' Cannot leave dashboard: not connected');
      return Promise.resolve();
    }

    return this.hubConnection.invoke('LeaveDashboard')
      .then(() => console.log(' Left dashboard'))
      .catch(err => console.error(' Error leaving dashboard:', err));
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
