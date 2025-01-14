using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Infrastructure.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class changePropertyOfIdPnan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MealPlanId",
                table: "Profiles");

            migrationBuilder.AddColumn<bool>(
                name: "ThereIsMealPlan",
                table: "Profiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThereIsMealPlan",
                table: "Profiles");

            migrationBuilder.AddColumn<string>(
                name: "MealPlanId",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
