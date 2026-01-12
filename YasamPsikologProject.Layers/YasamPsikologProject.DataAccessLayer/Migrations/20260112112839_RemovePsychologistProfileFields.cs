using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YasamPsikologProject.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemovePsychologistProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Biography",
                table: "Psychologists");

            migrationBuilder.DropColumn(
                name: "Certifications",
                table: "Psychologists");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "Psychologists");

            migrationBuilder.DropColumn(
                name: "ExperienceYears",
                table: "Psychologists");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "Psychologists",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Certifications",
                table: "Psychologists",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "Psychologists",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExperienceYears",
                table: "Psychologists",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
