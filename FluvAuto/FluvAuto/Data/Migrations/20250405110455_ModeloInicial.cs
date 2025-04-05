using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluvAuto.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModeloInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Morada = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NIF = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClienteId);
                });

            migrationBuilder.CreateTable(
                name: "Funcionarios",
                columns: table => new
                {
                    FuncionarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Funcao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionarios", x => x.FuncionarioId);
                });

            migrationBuilder.CreateTable(
                name: "Viaturas",
                columns: table => new
                {
                    ViaturaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Matricula = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    ClienteFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viaturas", x => x.ViaturaId);
                    table.ForeignKey(
                        name: "FK_Viaturas_Clientes_ClienteFK",
                        column: x => x.ClienteFK,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Marcacoes",
                columns: table => new
                {
                    MarcacaoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataMarcacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Servico = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ViaturaFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcacoes", x => x.MarcacaoId);
                    table.ForeignKey(
                        name: "FK_Marcacoes_Viaturas_ViaturaFK",
                        column: x => x.ViaturaFK,
                        principalTable: "Viaturas",
                        principalColumn: "ViaturaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DadosServicos",
                columns: table => new
                {
                    DadosId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HorasGastas = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MarcacaoFK = table.Column<int>(type: "int", nullable: false),
                    FuncionarioFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadosServicos", x => x.DadosId);
                    table.ForeignKey(
                        name: "FK_DadosServicos_Funcionarios_FuncionarioFK",
                        column: x => x.FuncionarioFK,
                        principalTable: "Funcionarios",
                        principalColumn: "FuncionarioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DadosServicos_Marcacoes_MarcacaoFK",
                        column: x => x.MarcacaoFK,
                        principalTable: "Marcacoes",
                        principalColumn: "MarcacaoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DadosServicos_FuncionarioFK",
                table: "DadosServicos",
                column: "FuncionarioFK");

            migrationBuilder.CreateIndex(
                name: "IX_DadosServicos_MarcacaoFK",
                table: "DadosServicos",
                column: "MarcacaoFK");

            migrationBuilder.CreateIndex(
                name: "IX_Marcacoes_ViaturaFK",
                table: "Marcacoes",
                column: "ViaturaFK");

            migrationBuilder.CreateIndex(
                name: "IX_Viaturas_ClienteFK",
                table: "Viaturas",
                column: "ClienteFK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DadosServicos");

            migrationBuilder.DropTable(
                name: "Funcionarios");

            migrationBuilder.DropTable(
                name: "Marcacoes");

            migrationBuilder.DropTable(
                name: "Viaturas");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
