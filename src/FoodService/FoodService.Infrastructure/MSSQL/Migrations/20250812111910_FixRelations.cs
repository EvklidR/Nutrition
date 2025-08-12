using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EatenFood_Dishes_DishId",
                table: "EatenFood");

            migrationBuilder.DropForeignKey(
                name: "FK_EatenFood_Products_ProductId",
                table: "EatenFood");

            migrationBuilder.DropIndex(
                name: "IX_EatenFood_DishId",
                table: "EatenFood");

            migrationBuilder.DropIndex(
                name: "IX_EatenFood_ProductId",
                table: "EatenFood");

            migrationBuilder.DropColumn(
                name: "DishId",
                table: "EatenFood");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "EatenFood");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DishId",
                table: "EatenFood",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "EatenFood",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EatenFood_DishId",
                table: "EatenFood",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_EatenFood_ProductId",
                table: "EatenFood",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_EatenFood_Dishes_DishId",
                table: "EatenFood",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EatenFood_Products_ProductId",
                table: "EatenFood",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
