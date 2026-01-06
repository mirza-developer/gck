using System;
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
            migrationBuilder.CreateTable(
                name: "tbl_Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BirthYear = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_FinancialAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_FinancialAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Table",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumberOfControllers = table.Column<int>(type: "int", nullable: false),
                    HourlyFeePerController = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsOccupied = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Table", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Session",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableId = table.Column<int>(type: "int", nullable: false),
                    FeePerHour = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    RecommendedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Session", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Session_tbl_Table_TableId",
                        column: x => x.TableId,
                        principalTable: "tbl_Table",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_AccountantReceipt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: false),
                    RecommendedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReceiptDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_AccountantReceipt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_AccountantReceipt_tbl_FinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "tbl_FinancialAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_AccountantReceipt_tbl_Session_SessionId",
                        column: x => x.SessionId,
                        principalTable: "tbl_Session",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_SessionCustomer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_SessionCustomer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_SessionCustomer_tbl_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tbl_Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_SessionCustomer_tbl_Session_SessionId",
                        column: x => x.SessionId,
                        principalTable: "tbl_Session",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_AccountantReceipt_FinancialAccountId",
                table: "tbl_AccountantReceipt",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_AccountantReceipt_ReceiptDateTime",
                table: "tbl_AccountantReceipt",
                column: "ReceiptDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_AccountantReceipt_SessionId",
                table: "tbl_AccountantReceipt",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Customer_PhoneNumber",
                table: "tbl_Customer",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Session_IsCompleted",
                table: "tbl_Session",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Session_StartDateTime",
                table: "tbl_Session",
                column: "StartDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Session_TableId",
                table: "tbl_Session",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_SessionCustomer_CustomerId",
                table: "tbl_SessionCustomer",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_SessionCustomer_SessionId_CustomerId",
                table: "tbl_SessionCustomer",
                columns: new[] { "SessionId", "CustomerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Table_Name",
                table: "tbl_Table",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_AccountantReceipt");

            migrationBuilder.DropTable(
                name: "tbl_SessionCustomer");

            migrationBuilder.DropTable(
                name: "tbl_FinancialAccount");

            migrationBuilder.DropTable(
                name: "tbl_Customer");

            migrationBuilder.DropTable(
                name: "tbl_Session");

            migrationBuilder.DropTable(
                name: "tbl_Table");
        }
    }
}
