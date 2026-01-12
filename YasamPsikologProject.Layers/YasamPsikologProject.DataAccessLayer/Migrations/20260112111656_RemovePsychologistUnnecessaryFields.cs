using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YasamPsikologProject.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemovePsychologistUnnecessaryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Psychologists_LicenseNumber",
                table: "Psychologists");

            migrationBuilder.DropColumn(
                name: "ConsultationDuration",
                table: "Psychologists");

            migrationBuilder.DropColumn(
                name: "ConsultationFee",
                table: "Psychologists");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Psychologists");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Psychologists");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConsultationDuration",
                table: "Psychologists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ConsultationFee",
                table: "Psychologists",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Psychologists",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "Psychologists",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Psychologists_LicenseNumber",
                table: "Psychologists",
                column: "LicenseNumber",
                unique: true);
        }
    }
}
