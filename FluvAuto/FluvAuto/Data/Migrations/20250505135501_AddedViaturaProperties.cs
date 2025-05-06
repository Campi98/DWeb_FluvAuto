using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluvAuto.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedViaturaProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Combustivel",
                table: "Viaturas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cor",
                table: "Viaturas",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Motorizacao",
                table: "Viaturas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Combustivel",
                table: "Viaturas");

            migrationBuilder.DropColumn(
                name: "Cor",
                table: "Viaturas");

            migrationBuilder.DropColumn(
                name: "Motorizacao",
                table: "Viaturas");
        }
    }
}
