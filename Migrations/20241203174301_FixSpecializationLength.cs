using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHospital.Migrations
{
    /// <inheritdoc />
    public partial class FixSpecializationLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Specialization",
                table: "Doctor",
                type: "nvarchar(2400)",
                maxLength: 2400,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Specialization",
                table: "Doctor",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2400)",
                oldMaxLength: 2400,
                oldNullable: true);
        }
    }
}
