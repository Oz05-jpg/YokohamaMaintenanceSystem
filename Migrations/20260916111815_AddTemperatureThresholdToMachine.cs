using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YokohamaMaintenanceSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTemperatureThresholdToMachine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TemperatureThreshold",
                table: "Machines",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemperatureThreshold",
                table: "Machines");
        }
    }
}
