using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATS_CV_Generator.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEducationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Educations",
                newName: "Major");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Educations",
                newName: "GradDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Major",
                table: "Educations",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "GradDate",
                table: "Educations",
                newName: "EndDate");
        }
    }
}
