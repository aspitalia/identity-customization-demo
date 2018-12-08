using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityCustomizationDemo.Data.Migrations
{
    public partial class AddedUserFiscalCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FiscalCode",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FiscalCode",
                table: "AspNetUsers");
        }
    }
}
