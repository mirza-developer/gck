using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gck.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnonymousCustomersCountToSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnonymousCustomersCount",
                table: "tbl_Session",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnonymousCustomersCount",
                table: "tbl_Session");
        }
    }
}
