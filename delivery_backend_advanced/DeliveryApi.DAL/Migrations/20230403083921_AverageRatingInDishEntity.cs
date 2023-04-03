using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApi.DAL.Migrations
{
    public partial class AverageRatingInDishEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Dishes",
                type: "double precision",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Dishes");
        }
    }
}
