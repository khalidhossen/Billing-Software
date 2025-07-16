using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class product_update30April25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
