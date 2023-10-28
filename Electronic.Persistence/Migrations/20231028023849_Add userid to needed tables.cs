using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Electronic.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Adduseridtoneededtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewerId",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReviewerId",
                table: "ReviewReplies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "NewItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "NewItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "NewItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "NewItems",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewerId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReviewerId",
                table: "ReviewReplies");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "NewItems");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "NewItems");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "NewItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "NewItems");
        }
    }
}
