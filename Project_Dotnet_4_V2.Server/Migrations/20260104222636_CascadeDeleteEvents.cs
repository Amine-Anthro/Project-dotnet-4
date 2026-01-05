using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Dotnet_4_V2.Server.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Rooms_RoomId",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "RoomId1",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_RoomId1",
                table: "Events",
                column: "RoomId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Rooms_RoomId",
                table: "Events",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Rooms_RoomId1",
                table: "Events",
                column: "RoomId1",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Rooms_RoomId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Rooms_RoomId1",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_RoomId1",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "RoomId1",
                table: "Events");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Rooms_RoomId",
                table: "Events",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
