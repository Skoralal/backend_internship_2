using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedExchangeRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cur");

            migrationBuilder.CreateTable(
                name: "exchange_rates",
                schema: "cur",
                columns: table => new
                {
                    actuality_time = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    exchange_rate = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exchange_rates", x => new { x.code, x.actuality_time });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exchange_rates",
                schema: "cur");
        }
    }
}
