using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gck.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReferralAndCreditSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ReferralCredit",
                table: "tbl_Customer",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReferralRewardPercentage",
                table: "tbl_Customer",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ReferredByCustomerId",
                table: "tbl_Customer",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedByAdmin",
                table: "tbl_Customer",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "tbl_CreditWithdrawalRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_CreditWithdrawalRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_CreditWithdrawalRequest_tbl_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tbl_Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Customer_IsVerifiedByAdmin",
                table: "tbl_Customer",
                column: "IsVerifiedByAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Customer_ReferredByCustomerId",
                table: "tbl_Customer",
                column: "ReferredByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_CreditWithdrawalRequest_CustomerId",
                table: "tbl_CreditWithdrawalRequest",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_CreditWithdrawalRequest_RequestDate",
                table: "tbl_CreditWithdrawalRequest",
                column: "RequestDate");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_CreditWithdrawalRequest_Status",
                table: "tbl_CreditWithdrawalRequest",
                column: "Status");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "tbl_Transaction",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_Customer_tbl_Customer_ReferredByCustomerId",
                table: "tbl_Customer",
                column: "ReferredByCustomerId",
                principalTable: "tbl_Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_Customer_tbl_Customer_ReferredByCustomerId",
                table: "tbl_Customer");

            migrationBuilder.DropTable(
                name: "tbl_CreditWithdrawalRequest");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "tbl_Transaction");

            migrationBuilder.DropIndex(
                name: "IX_tbl_Customer_IsVerifiedByAdmin",
                table: "tbl_Customer");

            migrationBuilder.DropIndex(
                name: "IX_tbl_Customer_ReferredByCustomerId",
                table: "tbl_Customer");

            migrationBuilder.DropColumn(
                name: "ReferralCredit",
                table: "tbl_Customer");

            migrationBuilder.DropColumn(
                name: "ReferralRewardPercentage",
                table: "tbl_Customer");

            migrationBuilder.DropColumn(
                name: "ReferredByCustomerId",
                table: "tbl_Customer");

            migrationBuilder.DropColumn(
                name: "IsVerifiedByAdmin",
                table: "tbl_Customer");
        }
    }
}
