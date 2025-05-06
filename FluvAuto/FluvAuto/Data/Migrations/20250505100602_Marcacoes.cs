using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluvAuto.Data.Migrations
{
    /// <inheritdoc />
    public partial class Marcacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "Marcacoes",
                newName: "Observacoes");

            migrationBuilder.RenameColumn(
                name: "DataMarcacao",
                table: "Marcacoes",
                newName: "DataPrevistaInicioServico");

            migrationBuilder.AlterColumn<string>(
                name: "Servico",
                table: "Marcacoes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFimServico",
                table: "Marcacoes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataMarcacaoFeita",
                table: "Marcacoes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataFimServico",
                table: "Marcacoes");

            migrationBuilder.DropColumn(
                name: "DataMarcacaoFeita",
                table: "Marcacoes");

            migrationBuilder.RenameColumn(
                name: "Observacoes",
                table: "Marcacoes",
                newName: "Descricao");

            migrationBuilder.RenameColumn(
                name: "DataPrevistaInicioServico",
                table: "Marcacoes",
                newName: "DataMarcacao");

            migrationBuilder.AlterColumn<string>(
                name: "Servico",
                table: "Marcacoes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
