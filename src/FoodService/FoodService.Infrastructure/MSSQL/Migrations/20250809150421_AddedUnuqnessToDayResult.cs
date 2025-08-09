using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodService.Infrastructure.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddedUnuqnessToDayResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DayResults_ProfileId_Date",
                table: "DayResults");

            migrationBuilder.CreateIndex(
                name: "IX_DayResults_ProfileId_Date",
                table: "DayResults",
                columns: new[] { "ProfileId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DayResults_ProfileId_Date",
                table: "DayResults");

            migrationBuilder.CreateIndex(
                name: "IX_DayResults_ProfileId_Date",
                table: "DayResults",
                columns: new[] { "ProfileId", "Date" });
        }
    }
}
