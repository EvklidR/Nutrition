using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EatenProducts",
                table: "EatenProducts");

            migrationBuilder.DropIndex(
                name: "IX_EatenProducts_ProductId",
                table: "EatenProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EatenDishes",
                table: "EatenDishes");

            migrationBuilder.DropIndex(
                name: "IX_EatenDishes_DishId",
                table: "EatenDishes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EatenProducts",
                table: "EatenProducts",
                columns: new[] { "ProductId", "MealId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EatenDishes",
                table: "EatenDishes",
                columns: new[] { "DishId", "MealId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EatenProducts",
                table: "EatenProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EatenDishes",
                table: "EatenDishes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EatenProducts",
                table: "EatenProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EatenDishes",
                table: "EatenDishes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EatenProducts_ProductId",
                table: "EatenProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_EatenDishes_DishId",
                table: "EatenDishes",
                column: "DishId");
        }
    }
}
