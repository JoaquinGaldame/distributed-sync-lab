using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OtaReplaceSimulator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSimulator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "external_properties",
                columns: table => new
                {
                    external_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    room_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    neighbourhood = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    latitude = table.Column<decimal>(type: "numeric(18,15)", precision: 18, scale: 15, nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(18,15)", precision: 18, scale: 15, nullable: true),
                    minimum_nights = table.Column<int>(type: "integer", nullable: true),
                    source_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_external_properties", x => x.external_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "external_properties");
        }
    }
}
