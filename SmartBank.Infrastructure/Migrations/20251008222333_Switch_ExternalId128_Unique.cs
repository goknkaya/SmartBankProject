using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartBank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Switch_ExternalId128_Unique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SwitchMessages_Acquirer_ExternalId",
                table: "SwitchMessages");

            migrationBuilder.DropIndex(
                name: "IX_SwitchMessages_Issuer_Status",
                table: "SwitchMessages");

            migrationBuilder.DropIndex(
                name: "IX_SwitchMessages_Status",
                table: "SwitchMessages");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalId",
                table: "SwitchMessages",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_ExternalId",
                table: "SwitchMessages",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_Issuer_Id",
                table: "SwitchMessages",
                columns: new[] { "Issuer", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_Status_Id",
                table: "SwitchMessages",
                columns: new[] { "Status", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SwitchMessages_ExternalId",
                table: "SwitchMessages");

            migrationBuilder.DropIndex(
                name: "IX_SwitchMessages_Issuer_Id",
                table: "SwitchMessages");

            migrationBuilder.DropIndex(
                name: "IX_SwitchMessages_Status_Id",
                table: "SwitchMessages");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalId",
                table: "SwitchMessages",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_Acquirer_ExternalId",
                table: "SwitchMessages",
                columns: new[] { "Acquirer", "ExternalId" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_Issuer_Status",
                table: "SwitchMessages",
                columns: new[] { "Issuer", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_Status",
                table: "SwitchMessages",
                column: "Status");
        }
    }
}
