using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fuse8.BackendInternship.PublicApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "user");

            migrationBuilder.CreateTable(
                name: "favorite_exchanges",
                schema: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    selected_currency_type = table.Column<int>(type: "integer", nullable: false),
                    base_currency_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_favorite_exchanges", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_favorite_exchanges_name",
                schema: "user",
                table: "favorite_exchanges",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_favorite_exchanges_selected_currency_type_base_currency_type",
                schema: "user",
                table: "favorite_exchanges",
                columns: new[] { "selected_currency_type", "base_currency_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "favorite_exchanges",
                schema: "user");
        }
    }
}
