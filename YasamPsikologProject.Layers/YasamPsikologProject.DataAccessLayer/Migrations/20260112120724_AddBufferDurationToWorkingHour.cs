using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YasamPsikologProject.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddBufferDurationToWorkingHour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BufferDuration",
                table: "WorkingHours",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BufferDuration",
                table: "WorkingHours");
        }
    }
}
