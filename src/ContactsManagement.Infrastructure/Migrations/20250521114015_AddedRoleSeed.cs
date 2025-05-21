using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ContactsManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedRoleSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("815b6a98-bd4e-4c08-9ac1-7c795452e498"), null, "Admin", "ADMIN" },
                    { new Guid("8e2af4f9-5b73-4e1d-9ad8-a83b12b4b397"), null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("815b6a98-bd4e-4c08-9ac1-7c795452e498"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8e2af4f9-5b73-4e1d-9ad8-a83b12b4b397"));
        }
    }
}
