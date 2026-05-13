using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTab.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMonobankInvoiceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MonobankInvoiceId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonobankInvoiceId",
                table: "Orders");
        }
    }
}
