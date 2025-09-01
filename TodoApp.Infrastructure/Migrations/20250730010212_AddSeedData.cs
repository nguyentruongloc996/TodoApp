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
            migrationBuilder.AlterColumn<string>(
                name: "GroupIds",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TaskIds",
                table: "Groups",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MemberIds",
                table: "Groups",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "MemberIds", "Name", "TaskIds" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), "11111111-1111-1111-1111-111111111111,22222222-2222-2222-2222-222222222222", "Test Group", "44444444-4444-4444-4444-444444444444" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "GroupIds", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "test1@example.com", "33333333-3333-3333-3333-333333333333", "Test User 1" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "test2@example.com", "33333333-3333-3333-3333-333333333333", "Test User 2" }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "Description", "DueDate", "GroupId", "Status", "UserId" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Complete project documentation", new DateTime(2025, 8, 6, 8, 2, 11, 538, DateTimeKind.Utc).AddTicks(8566), new Guid("33333333-3333-3333-3333-333333333333"), "Pending", new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Review code changes", new DateTime(2025, 8, 2, 8, 2, 11, 538, DateTimeKind.Utc).AddTicks(8583), null, "Pending", new Guid("22222222-2222-2222-2222-222222222222") }
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
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.AlterColumn<string>(
                name: "GroupIds",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TaskIds",
                table: "Groups",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "MemberIds",
                table: "Groups",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
