using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartBank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToCustomerAndCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "NationalId",
                table: "Customers",
                newName: "TCKN");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Customers",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Customers",
                newName: "FullName");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Customers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardIssuerBank",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "CardLimit",
                table: "Cards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CardProvider",
                table: "Cards",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CardStatusChangeReason",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Cards",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DailyLimit",
                table: "Cards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "FailedPinAttempts",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsVirtual",
                table: "Cards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedAt",
                table: "Cards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentCardId",
                table: "Cards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PinHash",
                table: "Cards",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TransactionLimit",
                table: "Cards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CardIssuerBank",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "CardLimit",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "CardProvider",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "CardStatusChangeReason",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "DailyLimit",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "FailedPinAttempts",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "IsVirtual",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "LastUsedAt",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "ParentCardId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "PinHash",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "TransactionLimit",
                table: "Cards");

            migrationBuilder.RenameColumn(
                name: "TCKN",
                table: "Customers",
                newName: "NationalId");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Customers",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Customers",
                newName: "FirstName");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Customers",
                type: "datetime2",
                nullable: true);
        }
    }
}
