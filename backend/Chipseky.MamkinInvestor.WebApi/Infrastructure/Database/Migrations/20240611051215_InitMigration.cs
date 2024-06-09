using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    coins_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    trading_pair = table.Column<string>(type: "text", nullable: false),
                    order_type = table.Column<int>(type: "integer", nullable: false),
                    forecasted_price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.order_id);
                });

            migrationBuilder.CreateTable(
                name: "trade_events",
                columns: table => new
                {
                    db_trade_event_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    type = table.Column<string>(type: "text", nullable: false),
                    data = table.Column<object>(type: "json", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trade_events", x => x.db_trade_event_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "trade_events");
        }
    }
}
