using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gck.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFreeSessionToSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFreeSession",
                table: "tbl_Session",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFreeSession",
                table: "tbl_Session");
        }
    }
}
