using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATS_CV_Generator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDatesToProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateRange",
                table: "Projects",
                newName: "StartDate");

            migrationBuilder.AddColumn<string>(
                name: "EndDate",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Projects",
                newName: "DateRange");
        }
    }
}
