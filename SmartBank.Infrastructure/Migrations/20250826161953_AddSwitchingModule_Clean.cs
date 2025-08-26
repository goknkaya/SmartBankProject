using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartBank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSwitchingModule_Clean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Cards_CardId",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "AcquirerRef",
                table: "Transactions",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CardBins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bin = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDomestic = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardBins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SwitchMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PANMasked = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    Bin = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Acquirer = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CardId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwitchMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SwitchMessages_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SwitchMessages_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SwitchLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    Stage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Level = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PayloadIn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayloadOut = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwitchLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SwitchLogs_SwitchMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "SwitchMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AcquirerRef",
                table: "Transactions",
                column: "AcquirerRef");

            migrationBuilder.CreateIndex(
                name: "IX_ClearingRecords_BatchId_TransactionId",
                table: "ClearingRecords",
                columns: new[] { "BatchId", "TransactionId" },
                unique: true,
                filter: "[TransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CardBins_Bin",
                table: "CardBins",
                column: "Bin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SwitchLogs_CreatedAt",
                table: "SwitchLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchLogs_MessageId",
                table: "SwitchLogs",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_Acquirer_ExternalId",
                table: "SwitchMessages",
                columns: new[] { "Acquirer", "ExternalId" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_CardId",
                table: "SwitchMessages",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_CreatedAt",
                table: "SwitchMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_Issuer_Status",
                table: "SwitchMessages",
                columns: new[] { "Issuer", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_Status",
                table: "SwitchMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_TransactionId",
                table: "SwitchMessages",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Cards_CardId",
                table: "Transactions",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Cards_CardId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "CardBins");

            migrationBuilder.DropTable(
                name: "SwitchLogs");

            migrationBuilder.DropTable(
                name: "SwitchMessages");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AcquirerRef",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_ClearingRecords_BatchId_TransactionId",
                table: "ClearingRecords");

            migrationBuilder.DropColumn(
                name: "AcquirerRef",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Cards_CardId",
                table: "Transactions",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
