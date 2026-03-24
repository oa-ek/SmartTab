using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTab.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Key",
                table: "ProductSpecifications",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BuildParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PcId = table.Column<int>(type: "int", nullable: false),
                    ComponentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildParts_Products_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuildParts_Products_PcId",
                        column: x => x.PcId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildParts_ComponentId",
                table: "BuildParts",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildParts_PcId",
                table: "BuildParts",
                column: "PcId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildParts");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductSpecifications",
                newName: "Key");
        }
    }
}
