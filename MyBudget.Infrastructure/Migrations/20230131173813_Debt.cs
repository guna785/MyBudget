using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBudget.Infrastructure.Migrations
{
    public partial class Debt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaidEMIs",
                table: "Debts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RemainingEMIs",
                table: "Debts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidEMIs",
                table: "Debts");

            migrationBuilder.DropColumn(
                name: "RemainingEMIs",
                table: "Debts");
        }
    }
}
