using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodService.Infrastructure.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddedConstraintToDayResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DayResults_ProfileId_Date",
                table: "DayResults",
                columns: new[] { "ProfileId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DayResults_ProfileId_Date",
                table: "DayResults");
        }
    }
}
