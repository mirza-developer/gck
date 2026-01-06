using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gck.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGamingCenterManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_tbl_SessionCustomer_SessionId_CustomerId",
                table: "tbl_SessionCustomer",
                newName: "IX_SessionCustomer_SessionId_CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_SessionCustomer_SessionId_CustomerId",
                table: "tbl_SessionCustomer",
                newName: "IX_tbl_SessionCustomer_SessionId_CustomerId");
        }
    }
}
