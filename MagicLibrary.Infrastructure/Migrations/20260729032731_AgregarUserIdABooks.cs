using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicLibrary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarUserIdABooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Books");
        }
    }
}
