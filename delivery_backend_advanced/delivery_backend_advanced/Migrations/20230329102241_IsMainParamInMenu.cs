using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delivery_backend_advanced.Migrations
{
    public partial class IsMainParamInMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "Menus",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "Menus");
        }
    }
}
