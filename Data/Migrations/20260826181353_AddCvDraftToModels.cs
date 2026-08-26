using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATS_CV_Generator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCvDraftToModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_CvDrafts_CvDraftId",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_CvDrafts_CvDraftId",
                table: "Projects");

            migrationBuilder.AlterColumn<int>(
                name: "CvDraftId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CvDraftId",
                table: "Experiences",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_CvDrafts_CvDraftId",
                table: "Experiences",
                column: "CvDraftId",
                principalTable: "CvDrafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_CvDrafts_CvDraftId",
                table: "Projects",
                column: "CvDraftId",
                principalTable: "CvDrafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_CvDrafts_CvDraftId",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_CvDrafts_CvDraftId",
                table: "Projects");

            migrationBuilder.AlterColumn<int>(
                name: "CvDraftId",
                table: "Projects",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CvDraftId",
                table: "Experiences",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_CvDrafts_CvDraftId",
                table: "Experiences",
                column: "CvDraftId",
                principalTable: "CvDrafts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_CvDrafts_CvDraftId",
                table: "Projects",
                column: "CvDraftId",
                principalTable: "CvDrafts",
                principalColumn: "Id");
        }
    }
}
