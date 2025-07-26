using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicsStore.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 25, 4, 11, 18, 391, DateTimeKind.Utc).AddTicks(9000));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 25, 4, 11, 18, 391, DateTimeKind.Utc).AddTicks(9480));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 25, 4, 11, 18, 391, DateTimeKind.Utc).AddTicks(9490));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 25, 4, 11, 18, 391, DateTimeKind.Utc).AddTicks(9490));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 25, 4, 11, 18, 391, DateTimeKind.Utc).AddTicks(9490));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 25, 4, 11, 18, 391, DateTimeKind.Utc).AddTicks(9490));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 25, 4, 11, 18, 391, DateTimeKind.Utc).AddTicks(9490));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 25, 4, 11, 18, 391, DateTimeKind.Utc).AddTicks(9490));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(2880), new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(2880) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4170), new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4170) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4180), new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4180) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4180), new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4180) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4180), new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4180) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4470), new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4470) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4470), new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4470) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4470), new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4470) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4470), new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4470) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4470), new DateTime(2025, 7, 25, 4, 11, 18, 392, DateTimeKind.Utc).AddTicks(4480) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
