using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class errorfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Shifts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Punches",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "PayStubs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Availabilities",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_EmployeeId",
                table: "Shifts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Punches_EmployeeId",
                table: "Punches",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayStubs_EmployeeId",
                table: "PayStubs",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_EmployeeId",
                table: "Availabilities",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_AspNetUsers_EmployeeId",
                table: "Availabilities",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PayStubs_AspNetUsers_EmployeeId",
                table: "PayStubs",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Punches_AspNetUsers_EmployeeId",
                table: "Punches",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_AspNetUsers_EmployeeId",
                table: "Shifts",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_AspNetUsers_EmployeeId",
                table: "Availabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_PayStubs_AspNetUsers_EmployeeId",
                table: "PayStubs");

            migrationBuilder.DropForeignKey(
                name: "FK_Punches_AspNetUsers_EmployeeId",
                table: "Punches");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_AspNetUsers_EmployeeId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_EmployeeId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Punches_EmployeeId",
                table: "Punches");

            migrationBuilder.DropIndex(
                name: "IX_PayStubs_EmployeeId",
                table: "PayStubs");

            migrationBuilder.DropIndex(
                name: "IX_Availabilities_EmployeeId",
                table: "Availabilities");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Shifts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Punches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "PayStubs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Availabilities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
