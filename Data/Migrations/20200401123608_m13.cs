using Microsoft.EntityFrameworkCore.Migrations;

namespace sklad.Data.Migrations
{
    public partial class m13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_AspNetUsers_ApplicationUserId",
                table: "Address");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Address",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_AspNetUsers_ApplicationUserId",
                table: "Address",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_AspNetUsers_ApplicationUserId",
                table: "Address");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Address",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_AspNetUsers_ApplicationUserId",
                table: "Address",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
