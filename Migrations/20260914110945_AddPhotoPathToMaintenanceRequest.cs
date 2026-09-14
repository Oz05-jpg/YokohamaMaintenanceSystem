using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YokohamaMaintenanceSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoPathToMaintenanceRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorReadings_Machines_MachineId",
                table: "SensorReadings");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "SensorReadings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SensorReadings_Machines_MachineId",
                table: "SensorReadings",
                column: "MachineId",
                principalTable: "Machines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorReadings_Machines_MachineId",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "MaintenanceRequests");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "SensorReadings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorReadings_Machines_MachineId",
                table: "SensorReadings",
                column: "MachineId",
                principalTable: "Machines",
                principalColumn: "Id");
        }
    }
}
