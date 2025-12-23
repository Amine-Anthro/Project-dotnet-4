using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Dotnet_4_V2.Server.Migrations
{
    /// <inheritdoc />
    public partial class RenameNumberToNameRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Rooms",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Rooms",
                newName: "Number");
        }
    }
}
