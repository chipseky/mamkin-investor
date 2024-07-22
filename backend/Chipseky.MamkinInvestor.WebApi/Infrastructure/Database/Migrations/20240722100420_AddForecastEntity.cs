using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddForecastEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "forecast_id",
                table: "trades",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "forecasts",
                columns: table => new
                {
                    forecast_id = table.Column<Guid>(type: "uuid", nullable: false),
                    height_price = table.Column<decimal>(type: "numeric", nullable: false),
                    low_price = table.Column<decimal>(type: "numeric", nullable: false),
                    height_price_probability = table.Column<double>(type: "double precision", nullable: false),
                    low_price_probability = table.Column<double>(type: "double precision", nullable: false),
                    error = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_forecasts", x => x.forecast_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "forecasts");

            migrationBuilder.DropColumn(
                name: "forecast_id",
                table: "trades");
        }
    }
}
