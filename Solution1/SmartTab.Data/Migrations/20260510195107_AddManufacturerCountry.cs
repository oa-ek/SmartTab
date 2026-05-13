using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTab.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddManufacturerCountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Manufacturers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Country",
                value: null);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Country",
                value: null);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 3,
                column: "Country",
                value: null);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 4,
                column: "Country",
                value: null);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 5,
                column: "Country",
                value: null);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 6,
                column: "Country",
                value: null);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 7,
                column: "Country",
                value: null);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 8,
                column: "Country",
                value: null);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 9,
                column: "Country",
                value: null);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 10,
                column: "Country",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Manufacturers");
        }
    }
}
