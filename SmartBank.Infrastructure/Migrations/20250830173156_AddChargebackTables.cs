using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartBank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChargebackTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChargebackCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TransactionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReasonCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DisputedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplyBy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargebackCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargebackCases_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChargebackEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EvidenceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargebackEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargebackEvents_ChargebackCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "ChargebackCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChargebackCases_OpenedAt",
                table: "ChargebackCases",
                column: "OpenedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ChargebackCases_Status",
                table: "ChargebackCases",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ChargebackCases_TransactionId",
                table: "ChargebackCases",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargebackEvents_CaseId",
                table: "ChargebackEvents",
                column: "CaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargebackEvents");

            migrationBuilder.DropTable(
                name: "ChargebackCases");
        }
    }
}
