using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MenuInDishNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Menus_MenuEntityId",
                table: "Dishes");

            migrationBuilder.RenameColumn(
                name: "MenuEntityId",
                table: "Dishes",
                newName: "MenuId");

            migrationBuilder.RenameIndex(
                name: "IX_Dishes_MenuEntityId",
                table: "Dishes",
                newName: "IX_Dishes_MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Menus_MenuId",
                table: "Dishes",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Menus_MenuId",
                table: "Dishes");

            migrationBuilder.RenameColumn(
                name: "MenuId",
                table: "Dishes",
                newName: "MenuEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Dishes_MenuId",
                table: "Dishes",
                newName: "IX_Dishes_MenuEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Menus_MenuEntityId",
                table: "Dishes",
                column: "MenuEntityId",
                principalTable: "Menus",
                principalColumn: "Id");
        }
    }
}
