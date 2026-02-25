using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionDeTareas.Migrations
{
    /// <inheritdoc />
    public partial class CambiosCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categoria_AspNetUsers_IdUsuario",
                table: "Categoria");

            migrationBuilder.DropIndex(
                name: "IX_Categoria_IdUsuario",
                table: "Categoria");

            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "Categoria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdUsuario",
                table: "Categoria",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_IdUsuario",
                table: "Categoria",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Categoria_AspNetUsers_IdUsuario",
                table: "Categoria",
                column: "IdUsuario",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
