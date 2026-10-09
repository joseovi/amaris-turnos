using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmarisTurnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarActivoASucursal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Sucursales",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Sucursales");
        }
    }
}
