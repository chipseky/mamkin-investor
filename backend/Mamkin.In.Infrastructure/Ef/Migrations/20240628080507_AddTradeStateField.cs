using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mamkin.In.WebApi.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTradeStateField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "closed",
                table: "trades");

            migrationBuilder.RenameColumn(
                name: "trading_pair",
                table: "trades",
                newName: "symbol");

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "trades",
                type: "text",
                nullable: false,
                defaultValue: "Created");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "state",
                table: "trades");

            migrationBuilder.RenameColumn(
                name: "symbol",
                table: "trades",
                newName: "trading_pair");

            migrationBuilder.AddColumn<bool>(
                name: "closed",
                table: "trades",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
