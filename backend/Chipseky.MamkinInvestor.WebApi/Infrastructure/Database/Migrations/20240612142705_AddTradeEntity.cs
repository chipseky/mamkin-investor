using System;
using System.Collections.Generic;
using Chipseky.MamkinInvestor.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTradeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.CreateTable(
                name: "trades",
                columns: table => new
                {
                    trade_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trading_pair = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    held_coins_count = table.Column<decimal>(type: "numeric", nullable: false),
                    closed = table.Column<bool>(type: "boolean", nullable: false),
                    history = table.Column<List<TradeOrder>>(type: "json", nullable: false),
                    current_profit = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trades", x => x.trade_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trades");

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    coins_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    forecasted_price = table.Column<decimal>(type: "numeric", nullable: false),
                    order_type = table.Column<int>(type: "integer", nullable: false),
                    trading_pair = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.order_id);
                });
        }
    }
}
