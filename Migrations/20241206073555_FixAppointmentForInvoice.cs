using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHospital.Migrations
{
    /// <inheritdoc />
    public partial class FixAppointmentForInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Invoice_AppointmentId",
                table: "Invoice",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Appointment_AppointmentId",
                table: "Invoice",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "AppointmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Appointment_AppointmentId",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_AppointmentId",
                table: "Invoice");
        }
    }
}
