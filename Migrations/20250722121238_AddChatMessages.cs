using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicsStore.Migrations
{
    /// <inheritdoc />
    public partial class AddChatMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 12, 16, 20, 33, 637, DateTimeKind.Utc).AddTicks(6520));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 12, 16, 20, 33, 637, DateTimeKind.Utc).AddTicks(7030));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 12, 16, 20, 33, 637, DateTimeKind.Utc).AddTicks(7030));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 12, 16, 20, 33, 637, DateTimeKind.Utc).AddTicks(7030));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 12, 16, 20, 33, 637, DateTimeKind.Utc).AddTicks(7030));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 12, 16, 20, 33, 637, DateTimeKind.Utc).AddTicks(7030));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 12, 16, 20, 33, 637, DateTimeKind.Utc).AddTicks(7030));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 12, 16, 20, 33, 637, DateTimeKind.Utc).AddTicks(7030));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(710), new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(710) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(1940), new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(1940) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(1940), new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(1940) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(1940), new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(1940) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(1950), new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(1950) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(2240), new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(2240) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(2240), new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(2240) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(2250), new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(2250) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(2250), new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(2250) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(2250), new DateTime(2025, 7, 12, 16, 20, 33, 638, DateTimeKind.Utc).AddTicks(2250) });
        }
    }
}
