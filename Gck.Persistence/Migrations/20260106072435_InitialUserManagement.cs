using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gck.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialUserManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorIdentityID = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LastModifierIdentityID = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_UserClaim_tbl_User_UserId",
                        column: x => x.UserId,
                        principalTable: "tbl_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_User_Username",
                table: "tbl_User",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_UserClaim_UserId",
                table: "tbl_UserClaim",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_UserClaim");

            migrationBuilder.DropTable(
                name: "tbl_User");
        }
    }
}
