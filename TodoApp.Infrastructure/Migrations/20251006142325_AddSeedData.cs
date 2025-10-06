using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TodoApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DomainUsers",
                columns: new[] { "Id", "DisplayName" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Test User 1" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Test User 2" }
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), "Test Group" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DomainUserId", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 0, "911134a3-2a55-46b1-b98c-b324c09a4fe9", new Guid("11111111-1111-1111-1111-111111111111"), "test1@example.com", true, false, null, "TEST1@EXAMPLE.COM", "TEST USER 1", "AQAAAAIAAYagAAAAEBx8l2zY9dY8K+nJvQWg3ZnYq+4L5m9jX2pZ8nV7wQ3f0t1R5s6u9pA2bC3d4E5f6G7h8I9j", null, false, null, false, "Test User 1" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 0, "079a9f26-5998-4724-8fb4-1be5521c1060", new Guid("22222222-2222-2222-2222-222222222222"), "test2@example.com", true, false, null, "TEST2@EXAMPLE.COM", "TEST USER 2", "AQAAAAIAAYagAAAAECy9m3aZ0eZ9L+oKwRXh4aoZr+5M6n0kY3qA9oW8xR4g1u2S6t7v0qB3cD4e5F6g7H8i9J0k", null, false, null, false, "Test User 2" }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "Description", "DueDate", "GroupId", "Status", "UserId" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Complete project documentation", new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), "Pending", new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Review code changes", new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pending", new Guid("22222222-2222-2222-2222-222222222222") }
                });

            migrationBuilder.InsertData(
                table: "SubTasks",
                columns: new[] { "Id", "Description", "ParentTaskId", "Status" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666666666"), "Write API documentation", new Guid("44444444-4444-4444-4444-444444444444"), "Pending" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "Create user guide", new Guid("44444444-4444-4444-4444-444444444444"), "Completed" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "SubTasks",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "SubTasks",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "DomainUsers",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "DomainUsers",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));
        }
    }
}
