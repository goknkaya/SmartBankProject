using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartBank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChargebackUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargebackCases_Transactions_TransactionId",
                table: "ChargebackCases");

            migrationBuilder.DropIndex(
                name: "IX_ChargebackCases_OpenedAt",
                table: "ChargebackCases");

            migrationBuilder.DropIndex(
                name: "IX_ChargebackCases_Status",
                table: "ChargebackCases");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ChargebackEvents",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargebackCases_Transactions_TransactionId",
                table: "ChargebackCases",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargebackCases_Transactions_TransactionId",
                table: "ChargebackCases");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ChargebackEvents",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "getdate()");

            migrationBuilder.CreateIndex(
                name: "IX_ChargebackCases_OpenedAt",
                table: "ChargebackCases",
                column: "OpenedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ChargebackCases_Status",
                table: "ChargebackCases",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargebackCases_Transactions_TransactionId",
                table: "ChargebackCases",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");
        }
    }
}
