using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Infrastructure.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThereIsMealPlan",
                table: "Profiles");

            migrationBuilder.AddColumn<Guid>(
                name: "MealPlanId",
                table: "Profiles",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
