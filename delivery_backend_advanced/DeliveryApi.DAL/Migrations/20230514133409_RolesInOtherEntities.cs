using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RolesInOtherEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Ratings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                table: "Managers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "DishesInBasket",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                table: "Cooks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CookEntityOrderEntity",
                columns: table => new
                {
                    CookId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrdersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CookEntityOrderEntity", x => new { x.CookId, x.OrdersId });
                    table.ForeignKey(
                        name: "FK_CookEntityOrderEntity_Cooks_CookId",
                        column: x => x.CookId,
                        principalTable: "Cooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CookEntityOrderEntity_Orders_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourierEntityOrderEntity",
                columns: table => new
                {
                    CourierId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrdersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourierEntityOrderEntity", x => new { x.CourierId, x.OrdersId });
                    table.ForeignKey(
                        name: "FK_CourierEntityOrderEntity_Couriers_CourierId",
                        column: x => x.CourierId,
                        principalTable: "Couriers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourierEntityOrderEntity_Orders_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_CustomerId",
                table: "Ratings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_RestaurantId",
                table: "Managers",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_DishesInBasket_CustomerId",
                table: "DishesInBasket",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Cooks_RestaurantId",
                table: "Cooks",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_CookEntityOrderEntity_OrdersId",
                table: "CookEntityOrderEntity",
                column: "OrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_CourierEntityOrderEntity_OrdersId",
                table: "CourierEntityOrderEntity",
                column: "OrdersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cooks_Restaurants_RestaurantId",
                table: "Cooks",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DishesInBasket_Customers_CustomerId",
                table: "DishesInBasket",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_Restaurants_RestaurantId",
                table: "Managers",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Customers_CustomerId",
                table: "Ratings",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cooks_Restaurants_RestaurantId",
                table: "Cooks");

            migrationBuilder.DropForeignKey(
                name: "FK_DishesInBasket_Customers_CustomerId",
                table: "DishesInBasket");

            migrationBuilder.DropForeignKey(
                name: "FK_Managers_Restaurants_RestaurantId",
                table: "Managers");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Customers_CustomerId",
                table: "Ratings");

            migrationBuilder.DropTable(
                name: "CookEntityOrderEntity");

            migrationBuilder.DropTable(
                name: "CourierEntityOrderEntity");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_CustomerId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Managers_RestaurantId",
                table: "Managers");

            migrationBuilder.DropIndex(
                name: "IX_DishesInBasket_CustomerId",
                table: "DishesInBasket");

            migrationBuilder.DropIndex(
                name: "IX_Cooks_RestaurantId",
                table: "Cooks");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "DishesInBasket");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Cooks");
        }
    }
}
