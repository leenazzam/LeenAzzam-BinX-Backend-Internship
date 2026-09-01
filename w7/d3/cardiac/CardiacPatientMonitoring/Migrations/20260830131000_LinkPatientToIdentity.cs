using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardiacPatientMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class LinkPatientToIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_IdentityUserId",
                table: "Patients",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_IdentityUserId",
                table: "Patients",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_IdentityUserId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_IdentityUserId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Patients");
        }
    }
}
