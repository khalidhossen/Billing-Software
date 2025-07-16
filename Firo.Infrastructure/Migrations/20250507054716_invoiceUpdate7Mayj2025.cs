using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class invoiceUpdate7Mayj2025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceNumber",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Invoices");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceNumber",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
