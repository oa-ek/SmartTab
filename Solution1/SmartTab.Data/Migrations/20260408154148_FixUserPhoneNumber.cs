using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTab.Data.Migrations
{
    public partial class FixUserPhoneNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PhoneNumber та IsActive вже існують в БД (додані вручну раніше)
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}