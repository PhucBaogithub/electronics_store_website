using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicsStore.Migrations
{
    /// <inheritdoc />
    public partial class FixCartItemModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 13, 7, 401, DateTimeKind.Utc).AddTicks(9620));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(120));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(130));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(130));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(130));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(130));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(130));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(130));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(3440), new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(3440) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4660), new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4660) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4660), new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4660) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4660), new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4660) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4660), new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4660) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4960), new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4960) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4960), new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4960) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4960), new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4960) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4960), new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4960) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4960), new DateTime(2025, 6, 17, 20, 13, 7, 402, DateTimeKind.Utc).AddTicks(4970) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(1580));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(2060));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(2060));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(2060));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(2060));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(2060));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(2060));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(2060));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(5300), new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(5300) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6530), new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6530) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6590), new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6590) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6590), new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6590) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6590), new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6590) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6890), new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6890) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6890), new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6890) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6890), new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6890) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6890), new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6890) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6890), new DateTime(2025, 6, 17, 20, 12, 46, 706, DateTimeKind.Utc).AddTicks(6890) });
        }
    }
}
