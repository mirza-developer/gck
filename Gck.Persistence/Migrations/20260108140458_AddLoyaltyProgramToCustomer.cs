using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gck.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLoyaltyProgramToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLoyal",
                table: "tbl_Customer",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PaidSessionsCount",
                table: "tbl_Customer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SessionsRequiredForFree",
                table: "tbl_Customer",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLoyal",
                table: "tbl_Customer");

            migrationBuilder.DropColumn(
                name: "PaidSessionsCount",
                table: "tbl_Customer");

            migrationBuilder.DropColumn(
                name: "SessionsRequiredForFree",
                table: "tbl_Customer");
        }
    }
}
