using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Dotnet_4_V2.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddZonePricesToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EconomicPrice",
                table: "Rooms",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StandardPrice",
                table: "Rooms",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VipPrice",
                table: "Rooms",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EconomicPrice",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "StandardPrice",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "VipPrice",
                table: "Rooms");
        }
    }
}
