using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluvAuto.Data.Migrations
{
    /// <inheritdoc />
    public partial class Reset_DepoisDasSugestoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DadosServicos",
                table: "DadosServicos");

            migrationBuilder.DropIndex(
                name: "IX_DadosServicos_MarcacaoFK",
                table: "DadosServicos");

            migrationBuilder.DropColumn(
                name: "DadosId",
                table: "DadosServicos");

            migrationBuilder.RenameColumn(
                name: "FuncionarioId",
                table: "Funcionarios",
                newName: "UtilizadorId");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Clientes",
                newName: "UtilizadorId");

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Funcionarios",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "CodPostal",
                table: "Funcionarios",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fotografia",
                table: "Funcionarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Morada",
                table: "Funcionarios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Funcionarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicioServico",
                table: "DadosServicos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DadosServicos",
                table: "DadosServicos",
                columns: new[] { "MarcacaoFK", "FuncionarioFK" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DadosServicos",
                table: "DadosServicos");

            migrationBuilder.DropColumn(
                name: "CodPostal",
                table: "Funcionarios");

            migrationBuilder.DropColumn(
                name: "Fotografia",
                table: "Funcionarios");

            migrationBuilder.DropColumn(
                name: "Morada",
                table: "Funcionarios");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Funcionarios");

            migrationBuilder.DropColumn(
                name: "DataInicioServico",
                table: "DadosServicos");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "UtilizadorId",
                table: "Funcionarios",
                newName: "FuncionarioId");

            migrationBuilder.RenameColumn(
                name: "UtilizadorId",
                table: "Clientes",
                newName: "ClienteId");

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Funcionarios",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(18)",
                oldMaxLength: 18,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DadosId",
                table: "DadosServicos",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DadosServicos",
                table: "DadosServicos",
                column: "DadosId");

            migrationBuilder.CreateIndex(
                name: "IX_DadosServicos_MarcacaoFK",
                table: "DadosServicos",
                column: "MarcacaoFK");
        }
    }
}
