using FluvAuto.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FluvAuto.Data;

/// <summary>
/// Classe que representa o contexto da base de dados da aplicação
/// (Equivalente a CREATE SCHEMA em SQL)
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ############## Aqui especificam-se as tabelas associadas à BD ##############

    /// <summary>
    /// Tabela que representa os clientes da oficina
    /// </summary>
    public DbSet<Cliente> Clientes { get; set; }

    /// <summary>
    /// Tabela que representa os funcionários da oficina
    /// </summary>
    public DbSet<Funcionario> Funcionarios { get; set; }

    /// <summary>
    /// Tabela que representa os serviços realizados na oficina
    /// </summary>
    public DbSet<FuncionariosMarcacoes> DadosServicos { get; set; }

    /// <summary>
    /// Tabela que representa as marcações de serviços na oficina
    /// </summary>
    public DbSet<Marcacao> Marcacoes { get; set; }

    /// <summary>
    /// Tabela que representa as viaturas dos clientes
    /// </summary>
    public DbSet<Viatura> Viaturas { get; set; }

}
