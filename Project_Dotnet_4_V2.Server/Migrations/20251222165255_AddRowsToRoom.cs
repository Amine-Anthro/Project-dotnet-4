using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Dotnet_4_V2.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddRowsToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeatsPerRow",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRows",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatsPerRow",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "TotalRows",
                table: "Rooms");
        }
    }
}
