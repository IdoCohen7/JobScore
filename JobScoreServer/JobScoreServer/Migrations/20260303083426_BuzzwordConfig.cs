using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobScoreServer.Migrations
{
    /// <inheritdoc />
    public partial class BuzzwordConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Buzzword",
                table: "Violations");

            migrationBuilder.CreateTable(
                name: "Buzzwords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buzzwords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buzzwords_Name",
                table: "Buzzwords",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Buzzwords");

            migrationBuilder.AddColumn<string>(
                name: "Buzzword",
                table: "Violations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
