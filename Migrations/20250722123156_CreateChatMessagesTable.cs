using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicsStore.Migrations
{
    /// <inheritdoc />
    public partial class CreateChatMessagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConversationId = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    IsFromUser = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    MessageType = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductData = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(4270));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(4750));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(4760));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(4760));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(4760));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(4760));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(4810));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(4810));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(8400), new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(8400) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(9700), new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(9700) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(9700), new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(9700) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(9700), new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(9700) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(9710), new DateTime(2025, 7, 22, 12, 31, 56, 249, DateTimeKind.Utc).AddTicks(9710) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 31, 56, 250, DateTimeKind.Utc).AddTicks(20), new DateTime(2025, 7, 22, 12, 31, 56, 250, DateTimeKind.Utc).AddTicks(20) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 31, 56, 250, DateTimeKind.Utc).AddTicks(20), new DateTime(2025, 7, 22, 12, 31, 56, 250, DateTimeKind.Utc).AddTicks(20) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 31, 56, 250, DateTimeKind.Utc).AddTicks(20), new DateTime(2025, 7, 22, 12, 31, 56, 250, DateTimeKind.Utc).AddTicks(20) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 31, 56, 250, DateTimeKind.Utc).AddTicks(20), new DateTime(2025, 7, 22, 12, 31, 56, 250, DateTimeKind.Utc).AddTicks(20) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 31, 56, 250, DateTimeKind.Utc).AddTicks(30), new DateTime(2025, 7, 22, 12, 31, 56, 250, DateTimeKind.Utc).AddTicks(30) });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 12, 37, 653, DateTimeKind.Utc).AddTicks(9160));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 12, 37, 653, DateTimeKind.Utc).AddTicks(9640));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 12, 37, 653, DateTimeKind.Utc).AddTicks(9640));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 12, 37, 653, DateTimeKind.Utc).AddTicks(9650));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 12, 37, 653, DateTimeKind.Utc).AddTicks(9650));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 12, 37, 653, DateTimeKind.Utc).AddTicks(9650));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 12, 37, 653, DateTimeKind.Utc).AddTicks(9650));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 22, 12, 12, 37, 653, DateTimeKind.Utc).AddTicks(9670));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(3090), new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(3090) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4320), new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4320) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4330), new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4330) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4330), new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4330) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4330), new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4330) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4620), new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4620) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4630), new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4630) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4630), new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4630) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4630), new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4630) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4630), new DateTime(2025, 7, 22, 12, 12, 37, 654, DateTimeKind.Utc).AddTicks(4630) });
        }
    }
}
