using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddForecastedOffset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "forecasted_sell_date",
                table: "trades",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "forecasted_sell_offset",
                table: "selected_symbols",
                type: "interval",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "forecasted_sell_date",
                table: "trades");

            migrationBuilder.DropColumn(
                name: "forecasted_sell_offset",
                table: "selected_symbols");
        }
    }
}
