using Microsoft.EntityFrameworkCore;
using Project_Dotnet_4_V2.Data;
using Project_Dotnet_4_V2.Models;
using Project_Dotnet_4_V2.Server.Repositories;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;

namespace Project_Dotnet_4_V2.Server.Services
{
    public class TicketService : ITicketService
    {
        private readonly IReservationRepository _reservationRepository;
        

        public TicketService(IReservationRepository reservationRepository) 
        {
            _reservationRepository = reservationRepository;
          
            QuestPDF.Settings.License = LicenseType.Community;
        }
        public async Task<string> GenerateQRCodeAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
                return string.Empty;

            var qrData = new
            {
                Type = "MisterTicket",
                ReservationId = reservation.Id,
                PaymentRef = reservation.Payment?.Reference ?? "N/A",
                Amount = reservation.TotalAmount,
                Customer = reservation.User?.Username,
                Event = reservation.Event?.Name,
                Date = reservation.Event?.Date.ToString("yyyy-MM-dd HH:mm"),
                Seats = reservation.Seats.Select(s => $"{s.Row}{s.Number}").ToList()
            };

            string qrContent = JsonSerializer.Serialize(qrData);

            using (QRCodeGenerator qrCodeGenerator = new QRCodeGenerator())
            {
                QRCodeData qRCodeData = qrCodeGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                using (PngByteQRCode qRCode = new PngByteQRCode(qRCodeData))
                {
                    byte[] qrCodeBytes = qRCode.GetGraphic(20);
                    return Convert.ToBase64String(qrCodeBytes);
                }
            }
        }

       

         public async Task<byte[]> GenerateTicketPdfAsync(int reservationId)
         {
             var reservation = await _reservationRepository.GetByIdAsync(reservationId);

             if (reservation == null)
             {
                 throw new KeyNotFoundException($"Reservation {reservationId} not found");
             }

             if (reservation.Status != Project_Dotnet_4_V2.Models.ReservationStatus.Paid)
            {
                throw new InvalidOperationException("Only paid reservations can get pdf tickets");
            }



            string qrCodeBase64 = await GenerateQRCodeAsync(reservationId);
            byte[] qrCodeBytes = Convert.FromBase64String(qrCodeBase64);


            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Height(100)
                        .Background(Colors.Blue.Darken3)
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("Cinepolis - Electronic tiket")
                        .FontSize(24)
                        .FontColor(Colors.White)
                        .Bold();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(col =>
                        {
                            
                            col.Item().Text($"Reservation #{reservation.Id}")
                                .FontSize(18)
                                .Bold();

                            if (reservation.Payment != null)
                            {
                                col.Item().PaddingTop(5).Text($"Reference : {reservation.Payment.Reference}")
                                    .FontSize(12)
                                    .FontColor(Colors.Blue.Darken2);
                            }

                            col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            col.Item().PaddingTop(20).Row(row =>
                            {
                                row.RelativeItem().Column(column =>
                                {
                                    column.Item().Text("Event").Bold().FontSize(14);
                                    column.Item().PaddingTop(5).Text(reservation.Event?.Name ?? "N/A");

                                    column.Item().PaddingTop(15).Text("Date and hour").Bold().FontSize(14);
                                    column.Item().PaddingTop(5).Text(reservation.Event?.Date.ToString("dddd dd MMMM yyyy à HH:mm") ?? "N/A");

                                    column.Item().PaddingTop(15).Text("Room").Bold().FontSize(14);
                                    column.Item().PaddingTop(5).Text($"{reservation.Event?.Room?.Name ?? "N/A"}");

                                    column.Item().PaddingTop(15).Text("Titulaire").Bold().FontSize(14);
                                    column.Item().PaddingTop(5).Text(reservation.User?.Username ?? "N/A");
                                });

                                row.ConstantItem(200).Column(column =>
                                {
                                    column.Item().AlignCenter().Text("QR Code").Bold().FontSize(14);
                                    column.Item().PaddingTop(10).AlignCenter().Image(qrCodeBytes).FitWidth();
                                });
                            });

                            col.Item().PaddingTop(30).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            
                            col.Item().PaddingTop(20).Text("Reserved seats").Bold().FontSize(16);
                            col.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(100);
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(100);
                                });

                                
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Seat").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Zone").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Price").Bold();
                                });

                                
                                foreach (var seat in reservation.Seats)
                                {
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{seat.Row}{seat.Number}");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(seat.Zone.ToString());
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{seat.Price:F2}€");
                                }
                            });

                            col.Item().PaddingTop(20).AlignRight().Text($"Total paid : {reservation.TotalAmount:F2}€")
                                .FontSize(18)
                                .Bold()
                                .FontColor(Colors.Blue.Darken3);

                            if (reservation.Payment != null)
                            {
                                col.Item().PaddingTop(5).AlignRight().Text($"Payment : {reservation.Payment.Reference}")
                                    .FontSize(12)
                                    .FontColor(Colors.Grey.Darken1);
                            }

                            col.Item().PaddingTop(30).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                           
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated at ");
                            x.Span(DateTime.Now.ToString("dd/MM/yyyy à HH:mm")).Bold();
                            x.Span(" - Cinepolis © 2025");
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
  }

