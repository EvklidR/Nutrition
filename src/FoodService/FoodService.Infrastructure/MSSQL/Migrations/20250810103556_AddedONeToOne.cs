using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodService.Infrastructure.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddedONeToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Food_DishId",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_DishId",
                table: "Recipes");

            migrationBuilder.CreateIndex(
                name: "IX_Food_RecipeId",
                table: "Food",
                column: "RecipeId",
                unique: true,
                filter: "[RecipeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Food_Recipes_RecipeId",
                table: "Food",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Food_Recipes_RecipeId",
                table: "Food");

            migrationBuilder.DropIndex(
                name: "IX_Food_RecipeId",
                table: "Food");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_DishId",
                table: "Recipes",
                column: "DishId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Food_DishId",
                table: "Recipes",
                column: "DishId",
                principalTable: "Food",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
