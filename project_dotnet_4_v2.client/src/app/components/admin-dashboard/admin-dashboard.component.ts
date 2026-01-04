import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { DashboardStats, DashboardService, EventStats } from '../../services/dashboard.service';
import { SignalrService } from '../../services/signalr.service';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit, OnDestroy {
  stats?: DashboardStats;
  isLoading = true;
  errorMessage = '';

  private dashboardUpdateSub?: Subscription;
  private eventStatsUpdateSub?: Subscription;

  constructor(
    private dashboardService: DashboardService,
    private signalrService: SignalrService
  ) { }

  async ngOnInit(): Promise<void> {
    await this.loadStats();
    await this.connectToSignalR();
  }

  ngOnDestroy(): void {
    this.dashboardUpdateSub?.unsubscribe();
    this.eventStatsUpdateSub?.unsubscribe();
    this.signalrService.leaveDashboard();
  }

  async loadStats(): Promise<void> {
    this.dashboardService.getDashboardStats().subscribe({
      next: (data) => {
        this.stats = data;
        this.isLoading = false;
        console.log('Initial stats loaded:', this.stats);
      },
      error: (error) => {
        console.error('Error when loading the stats', error);
        this.errorMessage = 'Impossible to load the stats';
        this.isLoading = false;
      }
    });
  }

  async connectToSignalR(): Promise<void> {
    try {
      await this.signalrService.startConnection();
      await this.signalrService.joinDashboard();

      this.dashboardUpdateSub = this.signalrService.dashboardUpdated$.subscribe({
        next: (updatedStats: DashboardStats) => {
          console.log('BEFORE UPDATE:', this.stats);
          this.stats = updatedStats;
          console.log('AFTER UPDATE:', this.stats);
        }
      });

      this.eventStatsUpdateSub = this.signalrService.eventStatsUpdated$.subscribe({
        next: (eventStats: EventStats) => {
          console.log('Event stats received:', eventStats);
          this.updateEventStats(eventStats);
        }
      });

      console.log('Dashboard SignalR connected');
    } catch (error) {
      console.error('Dashboard SignalR error:', error);
    }
  }

  updateEventStats(updatedEventStats: EventStats): void {
    if (!this.stats) {
      console.warn('No stats available');
      return;
    }

    console.log('Current eventsStats:', this.stats.eventsStats);

    const index = this.stats.eventsStats.findIndex(e => e.eventId === updatedEventStats.eventId);
    console.log('Found index:', index);

    if (index !== -1) {
      const newEventsStats = [...this.stats.eventsStats];
      newEventsStats[index] = updatedEventStats;

      const totalSeatsReserved = newEventsStats.reduce((sum, e) => sum + e.reservedSeats, 0);
      const totalSeatsPaid = newEventsStats.reduce((sum, e) => sum + e.paidSeats, 0);
      const totalRevenue = newEventsStats.reduce((sum, e) => sum + e.revenue, 0);
      const potentialTotal = newEventsStats.reduce((sum, e) => sum + e.potentialRevenue, 0);

      this.stats = {
        totalEvents: this.stats.totalEvents,
        totalReservations: this.stats.totalReservations,
        totalSeatsReserved,
        totalSeatsPaid,
        totalRevenue,
        pendingRevenue: potentialTotal - totalRevenue,
        eventsStats: newEventsStats
      };

      console.log('Stats UPDATED:', this.stats);
    } else {
      console.warn('Event not found in stats:', updatedEventStats.eventId);
    }
  }

  getStatusClass(fillRate: number): string {
    if (fillRate >= 80) return 'status-high';
    if (fillRate >= 50) return 'status-medium';
    return 'status-low';
  }
}
