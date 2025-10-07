using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartBank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSignatureHashColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClearingRecords_BatchId_TransactionId",
                table: "ClearingRecords");

            migrationBuilder.DropIndex(
                name: "IX_ClearingBatches_FileHash",
                table: "ClearingBatches");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Transactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "SignatureHash",
                table: "Transactions",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureHash",
                table: "ClearingRecords",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SignatureHash",
                table: "Transactions",
                column: "SignatureHash");

            migrationBuilder.CreateIndex(
                name: "IX_ClearingRecords_BatchId_LineNumber",
                table: "ClearingRecords",
                columns: new[] { "BatchId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClearingRecords_BatchId_TransactionId",
                table: "ClearingRecords",
                columns: new[] { "BatchId", "TransactionId" },
                unique: true,
                filter: "[TransactionId] IS NOT NULL AND [MatchStatus] = 'M'");

            migrationBuilder.CreateIndex(
                name: "IX_ClearingRecords_CardLast4_Amount_Currency_TransactionDate_MerchantName",
                table: "ClearingRecords",
                columns: new[] { "CardLast4", "Amount", "Currency", "TransactionDate", "MerchantName" });

            migrationBuilder.CreateIndex(
                name: "IX_ClearingRecords_SignatureHash",
                table: "ClearingRecords",
                column: "SignatureHash");

            migrationBuilder.CreateIndex(
                name: "IX_ClearingBatches_Direction_FileHash",
                table: "ClearingBatches",
                columns: new[] { "Direction", "FileHash" },
                unique: true,
                filter: "[Direction] = 'IN' AND [FileHash] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClearingBatches_Direction_SettlementDate_CreatedAt",
                table: "ClearingBatches",
                columns: new[] { "Direction", "SettlementDate", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_SignatureHash",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_ClearingRecords_BatchId_LineNumber",
                table: "ClearingRecords");

            migrationBuilder.DropIndex(
                name: "IX_ClearingRecords_BatchId_TransactionId",
                table: "ClearingRecords");

            migrationBuilder.DropIndex(
                name: "IX_ClearingRecords_CardLast4_Amount_Currency_TransactionDate_MerchantName",
                table: "ClearingRecords");

            migrationBuilder.DropIndex(
                name: "IX_ClearingRecords_SignatureHash",
                table: "ClearingRecords");

            migrationBuilder.DropIndex(
                name: "IX_ClearingBatches_Direction_FileHash",
                table: "ClearingBatches");

            migrationBuilder.DropIndex(
                name: "IX_ClearingBatches_Direction_SettlementDate_CreatedAt",
                table: "ClearingBatches");

            migrationBuilder.DropColumn(
                name: "SignatureHash",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SignatureHash",
                table: "ClearingRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Transactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClearingRecords_BatchId_TransactionId",
                table: "ClearingRecords",
                columns: new[] { "BatchId", "TransactionId" },
                unique: true,
                filter: "[TransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClearingBatches_FileHash",
                table: "ClearingBatches",
                column: "FileHash",
                unique: true);
        }
    }
}
