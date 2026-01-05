import { Component, OnInit } from '@angular/core';
import { RoomService, Room, CreateRoomDTO } from '../../services/room.service';

@Component({
  selector: 'app-admin-rooms',
  templateUrl: './admin-rooms.component.html',
  styleUrls: ['./admin-rooms.component.css']
})
export class AdminRoomsComponent implements OnInit {
  rooms: Room[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  showForm = false;
  isEditing = false;
  currentRoomId?: number;

  roomForm: CreateRoomDTO = {
    name: '',
    capacity: '',
    totalRows: 10,
    seatsPerRow: 10,
    economicPrice: 10,
    vipPrice: 18,
    standardPrice: 14
  };

  constructor(private roomService: RoomService) { }

  ngOnInit(): void {
    this.loadRooms();
  }

  loadRooms(): void {
    this.isLoading = true;
    this.roomService.getRooms().subscribe({
      next: (data) => {
        this.rooms = data;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Error loading rooms';
        this.isLoading = false;
      }
    });
  }

  openCreateForm(): void {
    this.showForm = true;
    this.isEditing = false;
    this.resetForm();
  }

  openEditForm(room: Room): void {
    this.showForm = true;
    this.isEditing = true;
    this.currentRoomId = room.id;
    this.roomForm = {
      name: room.name,
      capacity: room.capacity,
      totalRows: room.totalRows,
      seatsPerRow: room.seatsPerRow,
      economicPrice: room.economicPrice,
      vipPrice: room.vipPrice,
      standardPrice: room.standardPrice
    };
  }

  closeForm(): void {
    this.showForm = false;
    this.resetForm();
  }

  resetForm(): void {
    this.roomForm = {
      name: '',
      capacity: '',
      totalRows: 10,
      seatsPerRow: 10,
      economicPrice: 10,
      vipPrice: 18,
      standardPrice: 14
    };
    this.currentRoomId = undefined;
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isEditing && this.currentRoomId) {
      this.roomService.updateRoom(this.currentRoomId, this.roomForm).subscribe({
        next: () => {
          this.successMessage = 'Room updated successfully';
          this.closeForm();
          this.loadRooms();
        },
        error: (error) => {
          this.errorMessage = 'Error updating room';
        }
      });
    } else {
      this.roomService.createRoom(this.roomForm).subscribe({
        next: () => {
          this.successMessage = 'Room created successfully';
          this.closeForm();
          this.loadRooms();
        },
        error: (error) => {
          this.errorMessage = 'Error creating room';
        }
      });
    }
  }

  deleteRoom(id: number, roomNumber: string): void {
    if (confirm(`Are you sure you want to delete room ${roomNumber}?`)) {
      this.roomService.deleteRoom(id).subscribe({
        next: () => {
          this.successMessage = 'Room deleted successfully';
          this.loadRooms();
        },
        error: (error) => {
          this.errorMessage = 'Error deleting room';

          if (error.error && error.error.message) {
            this.errorMessage = error.error.message;
          } else if (error.status === 404) {
            this.errorMessage = 'Room not found';
          } else if (error.status === 400) {
            this.errorMessage = error.error?.message || 'Cannot delete this room';
          } else {
            this.errorMessage = 'Error deleting room. Please try again.';
          }

          this.clearMessages();
        }
      });
    }
  }

  private clearMessages(): void {
    setTimeout(() => {
      this.successMessage = '';
      this.errorMessage = '';
    }, 5000);
  }
}
