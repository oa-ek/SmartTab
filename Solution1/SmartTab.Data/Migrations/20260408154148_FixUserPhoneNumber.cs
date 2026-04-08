using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTab.Data.Migrations
{
    public partial class FixUserPhoneNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Додаємо колонку PhoneNumber в таблицю Users
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true); // true, оскільки телефон не є обов'язковим полем
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Видаляємо колонку, якщо буде потрібно відкотити міграцію назад
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");
        }
    }
}