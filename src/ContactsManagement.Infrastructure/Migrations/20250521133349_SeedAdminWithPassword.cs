using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminWithPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("005db85d-8da6-4d5e-a6de-cb0de751cab7"), 0, "098a7ce3-4ba3-4945-96e4-8fd21da3b122", "admin@admin.com", true, "admiiiiin", false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAIAAYagAAAAENa7jS7sKmDxR6LpfOL7YD0NVTzTscinU4+udNaL+E9dT7qWYb2LJEVqWau764rlmQ==", "0123456789", true, "VYXA6SV5NKN6FNS465MJ3LMGECPCT2QF", false, "admin@admin.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("815b6a98-bd4e-4c08-9ac1-7c795452e498"), new Guid("005db85d-8da6-4d5e-a6de-cb0de751cab7") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("815b6a98-bd4e-4c08-9ac1-7c795452e498"), new Guid("005db85d-8da6-4d5e-a6de-cb0de751cab7") });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("005db85d-8da6-4d5e-a6de-cb0de751cab7"));
        }
    }
}
