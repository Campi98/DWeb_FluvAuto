using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluvAuto.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixNameFuncionariosMarcacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DadosServicos_Funcionarios_FuncionarioFK",
                table: "DadosServicos");

            migrationBuilder.DropForeignKey(
                name: "FK_DadosServicos_Marcacoes_MarcacaoFK",
                table: "DadosServicos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DadosServicos",
                table: "DadosServicos");

            migrationBuilder.RenameTable(
                name: "DadosServicos",
                newName: "FuncionariosMarcacoes");

            migrationBuilder.RenameIndex(
                name: "IX_DadosServicos_FuncionarioFK",
                table: "FuncionariosMarcacoes",
                newName: "IX_FuncionariosMarcacoes_FuncionarioFK");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Marcacoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Funcionarios",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(18)",
                oldMaxLength: 18,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Clientes",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(18)",
                oldMaxLength: 18,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FuncionariosMarcacoes",
                table: "FuncionariosMarcacoes",
                columns: new[] { "MarcacaoFK", "FuncionarioFK" });

            migrationBuilder.AddForeignKey(
                name: "FK_FuncionariosMarcacoes_Funcionarios_FuncionarioFK",
                table: "FuncionariosMarcacoes",
                column: "FuncionarioFK",
                principalTable: "Funcionarios",
                principalColumn: "UtilizadorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FuncionariosMarcacoes_Marcacoes_MarcacaoFK",
                table: "FuncionariosMarcacoes",
                column: "MarcacaoFK",
                principalTable: "Marcacoes",
                principalColumn: "MarcacaoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuncionariosMarcacoes_Funcionarios_FuncionarioFK",
                table: "FuncionariosMarcacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_FuncionariosMarcacoes_Marcacoes_MarcacaoFK",
                table: "FuncionariosMarcacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FuncionariosMarcacoes",
                table: "FuncionariosMarcacoes");

            migrationBuilder.RenameTable(
                name: "FuncionariosMarcacoes",
                newName: "DadosServicos");

            migrationBuilder.RenameIndex(
                name: "IX_FuncionariosMarcacoes_FuncionarioFK",
                table: "DadosServicos",
                newName: "IX_DadosServicos_FuncionarioFK");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Marcacoes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Funcionarios",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Clientes",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(18)",
                oldMaxLength: 18);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DadosServicos",
                table: "DadosServicos",
                columns: new[] { "MarcacaoFK", "FuncionarioFK" });

            migrationBuilder.AddForeignKey(
                name: "FK_DadosServicos_Funcionarios_FuncionarioFK",
                table: "DadosServicos",
                column: "FuncionarioFK",
                principalTable: "Funcionarios",
                principalColumn: "UtilizadorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DadosServicos_Marcacoes_MarcacaoFK",
                table: "DadosServicos",
                column: "MarcacaoFK",
                principalTable: "Marcacoes",
                principalColumn: "MarcacaoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
