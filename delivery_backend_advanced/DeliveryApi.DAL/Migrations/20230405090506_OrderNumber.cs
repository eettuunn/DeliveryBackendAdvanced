using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApi.DAL.Migrations
{
    public partial class OrderNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "Orders");
        }
    }
}
