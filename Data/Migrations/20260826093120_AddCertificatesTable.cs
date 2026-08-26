using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATS_CV_Generator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificatesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GitHubUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LinkedInUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfessionalTitle",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "CvDraftId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CvDraftId",
                table: "Experiences",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CvDraftId",
                table: "Educations",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CvDraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkedInUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GitHubUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfessionalSummary = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvDraft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CvDraft_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CredentialUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CvDraftId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificates_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Certificates_CvDraft_CvDraftId",
                        column: x => x.CvDraftId,
                        principalTable: "CvDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CvDraftId",
                table: "Projects",
                column: "CvDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_Experiences_CvDraftId",
                table: "Experiences",
                column: "CvDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_CvDraftId",
                table: "Educations",
                column: "CvDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_ApplicationUserId",
                table: "Certificates",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_CvDraftId",
                table: "Certificates",
                column: "CvDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_CvDraft_UserId",
                table: "CvDraft",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_CvDraft_CvDraftId",
                table: "Educations",
                column: "CvDraftId",
                principalTable: "CvDraft",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_CvDraft_CvDraftId",
                table: "Experiences",
                column: "CvDraftId",
                principalTable: "CvDraft",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_CvDraft_CvDraftId",
                table: "Projects",
                column: "CvDraftId",
                principalTable: "CvDraft",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educations_CvDraft_CvDraftId",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_CvDraft_CvDraftId",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_CvDraft_CvDraftId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "CvDraft");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CvDraftId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Experiences_CvDraftId",
                table: "Experiences");

            migrationBuilder.DropIndex(
                name: "IX_Educations_CvDraftId",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "CvDraftId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CvDraftId",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "CvDraftId",
                table: "Educations");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "GitHubUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfessionalTitle",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
