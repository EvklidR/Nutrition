using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodService.Infrastructure.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class ChangedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EatenFoods_Food_FoodId",
                table: "EatenFoods");

            migrationBuilder.DropForeignKey(
                name: "FK_EatenFoods_Meals_MealId",
                table: "EatenFoods");

            migrationBuilder.DropTable(
                name: "ProductsOfDishes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EatenFoods",
                table: "EatenFoods");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Food");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Food");

            migrationBuilder.RenameTable(
                name: "EatenFoods",
                newName: "EatenFood");

            migrationBuilder.RenameIndex(
                name: "IX_EatenFoods_MealId",
                table: "EatenFood",
                newName: "IX_EatenFood_MealId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Meals",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "RecipeId",
                table: "Food",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "DayResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "EatenFood",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "AmountOfPortions",
                table: "EatenFood",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EatenFood",
                table: "EatenFood",
                columns: new[] { "FoodId", "MealId" });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmountOfPortions = table.Column<int>(type: "int", nullable: false),
                    DishId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_Food_DishId",
                        column: x => x.DishId,
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductsOfRecipes",
                columns: table => new
                {
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeightInRecipe = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsOfRecipes", x => new { x.RecipeId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ProductsOfRecipes_Food_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductsOfRecipes_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductsOfRecipes_ProductId",
                table: "ProductsOfRecipes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_DishId",
                table: "Recipes",
                column: "DishId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EatenFood_Food_FoodId",
                table: "EatenFood",
                column: "FoodId",
                principalTable: "Food",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EatenFood_Meals_MealId",
                table: "EatenFood",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EatenFood_Food_FoodId",
                table: "EatenFood");

            migrationBuilder.DropForeignKey(
                name: "FK_EatenFood_Meals_MealId",
                table: "EatenFood");

            migrationBuilder.DropTable(
                name: "ProductsOfRecipes");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EatenFood",
                table: "EatenFood");

            migrationBuilder.DropColumn(
                name: "RecipeId",
                table: "Food");

            migrationBuilder.DropColumn(
                name: "AmountOfPortions",
                table: "EatenFood");

            migrationBuilder.RenameTable(
                name: "EatenFood",
                newName: "EatenFoods");

            migrationBuilder.RenameIndex(
                name: "IX_EatenFood_MealId",
                table: "EatenFoods",
                newName: "IX_EatenFoods_MealId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Meals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Food",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Food",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "DayResults",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "EatenFoods",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EatenFoods",
                table: "EatenFoods",
                columns: new[] { "FoodId", "MealId" });

            migrationBuilder.CreateTable(
                name: "ProductsOfDishes",
                columns: table => new
                {
                    DishId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeightPerPortion = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsOfDishes", x => new { x.DishId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ProductsOfDishes_Food_DishId",
                        column: x => x.DishId,
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductsOfDishes_Food_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductsOfDishes_ProductId",
                table: "ProductsOfDishes",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_EatenFoods_Food_FoodId",
                table: "EatenFoods",
                column: "FoodId",
                principalTable: "Food",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EatenFoods_Meals_MealId",
                table: "EatenFoods",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
