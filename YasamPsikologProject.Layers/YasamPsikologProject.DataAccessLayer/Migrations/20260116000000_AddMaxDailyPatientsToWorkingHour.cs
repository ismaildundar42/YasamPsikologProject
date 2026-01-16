using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YasamPsikologProject.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxDailyPatientsToWorkingHour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxDailyPatients",
                table: "WorkingHours",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDailyPatients",
                table: "WorkingHours");
        }
    }
}
