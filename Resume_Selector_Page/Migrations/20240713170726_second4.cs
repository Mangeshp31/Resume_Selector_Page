using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Resume_Selector_Page.Migrations
{
    /// <inheritdoc />
    public partial class second4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RecruiterId",
                table: "DownloadedResumes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_DownloadedResumes_RecruiterId",
                table: "DownloadedResumes",
                column: "RecruiterId");

            migrationBuilder.CreateIndex(
                name: "IX_DownloadedResumes_ResumeId",
                table: "DownloadedResumes",
                column: "ResumeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DownloadedResumes_AspNetUsers_RecruiterId",
                table: "DownloadedResumes",
                column: "RecruiterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DownloadedResumes_Resumes_ResumeId",
                table: "DownloadedResumes",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DownloadedResumes_AspNetUsers_RecruiterId",
                table: "DownloadedResumes");

            migrationBuilder.DropForeignKey(
                name: "FK_DownloadedResumes_Resumes_ResumeId",
                table: "DownloadedResumes");

            migrationBuilder.DropIndex(
                name: "IX_DownloadedResumes_RecruiterId",
                table: "DownloadedResumes");

            migrationBuilder.DropIndex(
                name: "IX_DownloadedResumes_ResumeId",
                table: "DownloadedResumes");

            migrationBuilder.AlterColumn<string>(
                name: "RecruiterId",
                table: "DownloadedResumes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
