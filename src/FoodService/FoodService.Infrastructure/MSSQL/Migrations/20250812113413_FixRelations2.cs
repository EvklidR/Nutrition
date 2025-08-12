using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRelations2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EatenFood_Dishes_FoodId",
                table: "EatenFood");

            migrationBuilder.DropForeignKey(
                name: "FK_EatenFood_Meals_MealId",
                table: "EatenFood");

            migrationBuilder.DropForeignKey(
                name: "FK_EatenFood_Products_FoodId",
                table: "EatenFood");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EatenFood",
                table: "EatenFood");

            migrationBuilder.DropColumn(
                name: "AmountOfPortions",
                table: "EatenFood");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "EatenFood");

            migrationBuilder.RenameTable(
                name: "EatenFood",
                newName: "EatenProducts");

            migrationBuilder.RenameColumn(
                name: "FoodId",
                table: "EatenProducts",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_EatenFood_MealId",
                table: "EatenProducts",
                newName: "IX_EatenProducts_MealId");

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "EatenProducts",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EatenProducts",
                table: "EatenProducts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EatenDishes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DishId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountOfPortions = table.Column<int>(type: "int", nullable: false),
                    MealId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EatenDishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EatenDishes_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EatenDishes_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EatenProducts_ProductId",
                table: "EatenProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_EatenDishes_DishId",
                table: "EatenDishes",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_EatenDishes_MealId",
                table: "EatenDishes",
                column: "MealId");

            migrationBuilder.AddForeignKey(
                name: "FK_EatenProducts_Meals_MealId",
                table: "EatenProducts",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EatenProducts_Products_ProductId",
                table: "EatenProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EatenProducts_Meals_MealId",
                table: "EatenProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_EatenProducts_Products_ProductId",
                table: "EatenProducts");

            migrationBuilder.DropTable(
                name: "EatenDishes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EatenProducts",
                table: "EatenProducts");

            migrationBuilder.DropIndex(
                name: "IX_EatenProducts_ProductId",
                table: "EatenProducts");

            migrationBuilder.RenameTable(
                name: "EatenProducts",
                newName: "EatenFood");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "EatenFood",
                newName: "FoodId");

            migrationBuilder.RenameIndex(
                name: "IX_EatenProducts_MealId",
                table: "EatenFood",
                newName: "IX_EatenFood_MealId");

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

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "EatenFood",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EatenFood",
                table: "EatenFood",
                columns: new[] { "FoodId", "MealId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EatenFood_Dishes_FoodId",
                table: "EatenFood",
                column: "FoodId",
                principalTable: "Dishes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EatenFood_Meals_MealId",
                table: "EatenFood",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EatenFood_Products_FoodId",
                table: "EatenFood",
                column: "FoodId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
