using Microsoft.EntityFrameworkCore.Migrations;

namespace sklad.Data.Migrations
{
    public partial class m19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Item");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Item",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Item");

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Item",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
