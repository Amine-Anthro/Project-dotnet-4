using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Dotnet_4_V2.Server.Services;
using Project_Dotnet_4_V2.DTO.Reservations;
using Project_Dotnet_4_V2.Models;
using System.Security.Claims;

namespace Project_Dotnet_4_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ITicketService _ticketService;
        public ReservationsController(IReservationService reservationService, ITicketService ticketService)
        {
            _reservationService = reservationService;
            _ticketService = ticketService;
        }

        /// <summary>
        /// Récupérer toutes les réservations (Admin uniquement)
        /// </summary>
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetAllReservations()
        {
            var reservations = await _reservationService.GetAllReservationsAsync();
            return Ok(reservations);
        }

        /// <summary>
        /// Récupérer les réservations de l'utilisateur connecté
        /// </summary>
        [Authorize]
        [HttpGet("my-reservations")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetMyReservations()
        {
            // Récupérer l'ID de l'utilisateur depuis le token JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            var reservations = await _reservationService.GetAllReservationsByUserIdAsync(userId);
            return Ok(reservations);
        }

        /// <summary>
        /// Récupérer les réservations d'un utilisateur spécifique (Admin uniquement)
        /// </summary>
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservationsByUserId(int userId)
        {
            var reservations = await _reservationService.GetAllReservationsByUserIdAsync(userId);
            return Ok(reservations);
        }

        /// <summary>
        /// Récupérer une réservation par ID
        /// </summary>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservationById(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);

            if (reservation == null)
                return NotFound(new { message = "Reservation not found" });

            // Vérifier que l'utilisateur est propriétaire ou admin
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            if (reservation.UserId != userId && userRole != "Admin")
                return Forbid(); // 403 Forbidden

            return Ok(reservation);
        }

        /// <summary>
        /// Créer une nouvelle réservation
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Reservation>> CreateReservation(CreateReservationDTO reservationDto)
        {
            try
            {
                // Vérifier que l'utilisateur réserve pour lui-même (sauf admin)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                if (reservationDto.UserId != userId && userRole != "Admin")
                {
                    return Forbid(new { message = "You can only create reservations for yourself" }.ToString());
                }

                var reservation = await _reservationService.CreateReservationAsync(reservationDto);
                return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Confirmer le paiement d'une réservation
        /// </summary>
        [Authorize]
        [HttpPost("{id}/confirm-payment")]
        public async Task<ActionResult<Reservation>> ConfirmPayment(int id)
        {
            try
            {
                var reservation = await _reservationService.GetReservationByIdAsync(id);

                if (reservation == null)
                    return NotFound(new { message = "Reservation not found" });

                // Vérifier que l'utilisateur est propriétaire ou admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                if (reservation.UserId != userId && userRole != "Admin")
                    return Forbid();

                var updatedReservation = await _reservationService.ConfirmPaymentAsync(id);
                return Ok(updatedReservation);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Annuler une réservation
        /// </summary>
        [Authorize]
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelReservation(int id)
        {
            try
            {
                var reservation = await _reservationService.GetReservationByIdAsync(id);

                if (reservation == null)
                    return NotFound(new { message = "Reservation not found" });

                // Vérifier que l'utilisateur est propriétaire ou admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                if (reservation.UserId != userId && userRole != "Admin")
                    return Forbid();

                await _reservationService.CancelReservationAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Télécharger le billet PDF
        /// </summary>
        [Authorize]
        [HttpGet("{id}/ticket/pdf")]
        public async Task<IActionResult> DownloadTicketPdf(int id)
        {
            try
            {
                var reservation = await _reservationService.GetReservationByIdAsync(id);

                if (reservation == null)
                    return NotFound(new { message = "Reservation not found" });

                // Vérifier que l'utilisateur est propriétaire ou admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                if (reservation.UserId != userId && userRole != "Admin")
                    return Forbid();

                var pdfBytes = await _ticketService.GenerateTicketPdfAsync(id);

                return File(pdfBytes, "application/pdf", $"Ticket-Reservation-{id}.pdf");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtenir le QR code en base64
        /// </summary>
        [Authorize]
        [HttpGet("{id}/ticket/qrcode")]
        public async Task<IActionResult> GetQRCode(int id)
        {
            try
            {
                var reservation = await _reservationService.GetReservationByIdAsync(id);

                if (reservation == null)
                    return NotFound(new { message = "Reservation not found" });

                // Vérifier que l'utilisateur est propriétaire ou admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                if (reservation.UserId != userId && userRole != "Admin")
                    return Forbid();

                var qrCodeBase64 = _ticketService.GenerateQRCode(id);

                return Ok(new { qrCode = qrCodeBase64 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating QR code", error = ex.Message });
            }
        }
    }
}