using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicLibrary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposHabitosMeta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiasPorSemana",
                table: "Goals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HoraNotificacion",
                table: "Goals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiasPorSemana",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "HoraNotificacion",
                table: "Goals");
        }
    }
}
