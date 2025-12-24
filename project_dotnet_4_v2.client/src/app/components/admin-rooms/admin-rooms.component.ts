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

  // pour le form
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
        this.errorMessage = 'Erreur lors du chargement des salles';
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
      // Modification
      this.roomService.updateRoom(this.currentRoomId, this.roomForm).subscribe({
        next: () => {
          this.successMessage = 'Salle modifiée avec succès';
          this.closeForm();
          this.loadRooms();
        },
        error: (error) => {
          this.errorMessage = 'Erreur lors de la modification';
        }
      });
    } else {
      // Création
      this.roomService.createRoom(this.roomForm).subscribe({
        next: () => {
          this.successMessage = 'Salle créée avec succès';
          this.closeForm();
          this.loadRooms();
        },
        error: (error) => {
          this.errorMessage = 'Erreur lors de la création';
        }
      });
    }
  }

  deleteRoom(id: number, roomNumber: string): void {
    if (confirm(`Êtes-vous sûr de vouloir supprimer la salle ${roomNumber} ?`)) {
      this.roomService.deleteRoom(id).subscribe({
        next: () => {
          this.successMessage = 'Salle supprimée avec succès';
          this.loadRooms();
        },
        error: (error) => {
          this.errorMessage = 'Erreur lors de la suppression';
        }
      });
    }
  }
}
