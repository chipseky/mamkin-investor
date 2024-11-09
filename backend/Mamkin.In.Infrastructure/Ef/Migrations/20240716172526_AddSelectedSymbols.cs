using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mamkin.In.WebApi.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectedSymbols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "selected_symbols",
                columns: table => new
                {
                    symbol = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_selected_symbols", x => x.symbol);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "selected_symbols");
        }
    }
}
