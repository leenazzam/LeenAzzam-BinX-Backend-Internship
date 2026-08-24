using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class SeedSprint1Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CreatedDate", "Description", "Name", "OwnerId" },
                values: new object[] { 2, new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Initial seeded project for Sprint 1", "Sample Project", "f0e5b283-ea85-4462-b567-d0f95f649ff8" });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "DueDate", "ProjectId", "Status", "Title" },
                values: new object[] { 1, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Pending", "Initial Setup Task" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
