using FluvAuto.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace FluvAuto.Data
{
    public static class DBInitializer
    {
        public static void Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Garantir que a base de dados existe
            context.Database.Migrate();

            // Seed do role admin
            var adminRole = "admin";
            if (!roleManager.RoleExistsAsync(adminRole).Result)
            {
                roleManager.CreateAsync(new IdentityRole(adminRole)).Wait();
            }

            // Seed do utilizador admin
            var adminEmail = "admin@admin.com";
            var adminUser = userManager.FindByEmailAsync(adminEmail).Result;
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = userManager.CreateAsync(adminUser, "Admin12345!").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(adminUser, adminRole).Wait();
                }
            }

            // Seed do role funcionario
            var funcionarioRole = "funcionario";
            if (!roleManager.RoleExistsAsync(funcionarioRole).Result)
            {
                roleManager.CreateAsync(new IdentityRole(funcionarioRole)).Wait();
            }

            // Seed Clientes
            if (!context.Clientes.Any())
            {
                // Cliente 1
                var identityUser1 = new IdentityUser { UserName = "joao@email.com", Email = "joao@email.com", EmailConfirmed = true };
                var result1 = userManager.CreateAsync(identityUser1, "Password123!").Result;
                if (result1.Succeeded)
                {
                    var cliente1 = new Cliente
                    {
                        Nome = "João Campos",
                        Email = identityUser1.Email,
                        Telefone = "912345678",
                        NIF = "123456789",
                        Morada = "Rua A, 1",
                        CodPostal = "1000-001 Lisboa",
                        UserName = identityUser1.UserName
                    };
                    context.Clientes.Add(cliente1);
                }
                // Cliente 2
                var identityUser2 = new IdentityUser { UserName = "ana@email.com", Email = "ana@email.com", EmailConfirmed = true };
                var result2 = userManager.CreateAsync(identityUser2, "Password123!").Result;
                if (result2.Succeeded)
                {
                    var cliente2 = new Cliente
                    {
                        Nome = "Ana Martins",
                        Email = identityUser2.Email,
                        Telefone = "913333333",
                        NIF = "987654321",
                        Morada = "Rua das Flores, 10",
                        CodPostal = "1100-222 Lisboa",
                        UserName = identityUser2.UserName
                    };
                    context.Clientes.Add(cliente2);
                }
                // Cliente 3
                var identityUser3 = new IdentityUser { UserName = "carlos@email.com", Email = "carlos@email.com", EmailConfirmed = true };
                var result3 = userManager.CreateAsync(identityUser3, "Password123!").Result;
                if (result3.Succeeded)
                {
                    var cliente3 = new Cliente
                    {
                        Nome = "Carlos Pinto",
                        Email = identityUser3.Email,
                        Telefone = "914444444",
                        NIF = "192837465",
                        Morada = "Av. Central, 50",
                        CodPostal = "4000-333 Porto",
                        UserName = identityUser3.UserName
                    };
                    context.Clientes.Add(cliente3);
                }
                context.SaveChanges();
            }

            // Seed Funcionários
            if (!context.Funcionarios.Any())
            {
                // Funcionário 1
                var identityUser1 = new IdentityUser { UserName = "maria@email.com", Email = "maria@email.com", EmailConfirmed = true };
                var result1 = userManager.CreateAsync(identityUser1, "Password123!").Result;
                if (result1.Succeeded)
                {
                    userManager.AddToRoleAsync(identityUser1, funcionarioRole).Wait();
                    var funcionario1 = new Funcionario
                    {
                        Nome = "Maria Sousa",
                        Email = identityUser1.Email,
                        Telefone = "934567890",
                        Funcao = "Mecânico",
                        Morada = "Rua B, 2",
                        CodPostal = "2000-002 Porto",
                        UserName = identityUser1.UserName
                    };
                    context.Funcionarios.Add(funcionario1);
                }
                // Funcionário 2
                var identityUser2 = new IdentityUser { UserName = "rui@email.com", Email = "rui@email.com", EmailConfirmed = true };
                var result2 = userManager.CreateAsync(identityUser2, "Password123!").Result;
                if (result2.Succeeded)
                {
                    userManager.AddToRoleAsync(identityUser2, funcionarioRole).Wait();
                    var funcionario2 = new Funcionario
                    {
                        Nome = "Rui Alves",
                        Email = identityUser2.Email,
                        Telefone = "935555555",
                        Funcao = "Rececionista",
                        Morada = "Rua Nova, 5",
                        CodPostal = "3000-444 Coimbra",
                        UserName = identityUser2.UserName
                    };
                    context.Funcionarios.Add(funcionario2);
                }
                // Funcionário 3
                var identityUser3 = new IdentityUser { UserName = "ines@email.com", Email = "ines@email.com", EmailConfirmed = true };
                var result3 = userManager.CreateAsync(identityUser3, "Password123!").Result;
                if (result3.Succeeded)
                {
                    userManager.AddToRoleAsync(identityUser3, funcionarioRole).Wait();
                    var funcionario3 = new Funcionario
                    {
                        Nome = "Inês Costa",
                        Email = identityUser3.Email,
                        Telefone = "936666666",
                        Funcao = "Gestora",
                        Morada = "Rua do Sol, 8",
                        CodPostal = "5000-555 Vila Real",
                        UserName = identityUser3.UserName
                    };
                    context.Funcionarios.Add(funcionario3);
                }
                context.SaveChanges();
            }

            // Seed Viaturas
            if (!context.Viaturas.Any())
            {
                var joao = context.Clientes.FirstOrDefault(c => c.Email == "joao@email.com");
                var ana = context.Clientes.FirstOrDefault(c => c.Email == "ana@email.com");
                var carlos = context.Clientes.FirstOrDefault(c => c.Email == "carlos@email.com");

                if (joao != null)
                {
                    context.Viaturas.AddRange(new[]
                    {
                        new Viatura {
                            Marca = "Renault", Modelo = "Clio", Matricula = "12-AB-34", Ano = 2018, Cor = "Vermelho", Combustivel = "Gasolina", Motorizacao = "1.2 TCe", ClienteFK = joao.UtilizadorId },
                        new Viatura {
                            Marca = "Peugeot", Modelo = "208", Matricula = "34-CD-56", Ano = 2020, Cor = "Azul", Combustivel = "Diesel", Motorizacao = "1.5 BlueHDi", ClienteFK = joao.UtilizadorId },
                        new Viatura {
                            Marca = "Volkswagen", Modelo = "Golf", Matricula = "78-EF-90", Ano = 2017, Cor = "Preto", Combustivel = "Gasolina", Motorizacao = "1.0 TSI", ClienteFK = joao.UtilizadorId }
                    });
                }
                if (ana != null)
                {
                    context.Viaturas.AddRange(new[]
                    {
                        new Viatura {
                            Marca = "Fiat", Modelo = "Panda", Matricula = "11-GH-22", Ano = 2015, Cor = "Branco", Combustivel = "GPL", Motorizacao = "0.9 TwinAir", ClienteFK = ana.UtilizadorId },
                        new Viatura {
                            Marca = "Toyota", Modelo = "Yaris", Matricula = "33-IJ-44", Ano = 2021, Cor = "Cinzento", Combustivel = "Híbrido", Motorizacao = "1.5 Hybrid", ClienteFK = ana.UtilizadorId }
                    });
                }
                if (carlos != null)
                {
                    context.Viaturas.Add(
                        new Viatura
                        {
                            Marca = "Ford",
                            Modelo = "Focus",
                            Matricula = "55-KL-66",
                            Ano = 2016,
                            Cor = "Prata",
                            Combustivel = "Diesel",
                            Motorizacao = "1.5 TDCi",
                            ClienteFK = carlos.UtilizadorId
                        }
                    );
                }
                context.SaveChanges();
            }

            // Seed Marcações e FuncionariosMarcacoes
            if (!context.Marcacoes.Any())
            {
                var viaturas = context.Viaturas.ToList();
                var funcionarios = context.Funcionarios.ToList();
                var marcacoes = new List<Marcacao>();
                var funcionariosMarcacoes = new List<FuncionariosMarcacoes>();
                var now = DateTime.Now;

                // Marcação 1 - João, Clio
                var marc1 = new Marcacao
                {
                    DataMarcacaoFeita = now.AddDays(-10),
                    DataPrevistaInicioServico = now.AddDays(-7),
                    DataFimServico = now.AddDays(-6),
                    Servico = "Revisão geral",
                    Observacoes = "Cliente pediu revisão completa.",
                    Estado = "Concluída",
                    ViaturaFK = viaturas.FirstOrDefault(v => v.Matricula == "12-AB-34")?.ViaturaId ?? 0
                };
                marcacoes.Add(marc1);

                // Marcação 2 - Ana, Yaris
                var marc2 = new Marcacao
                {
                    DataMarcacaoFeita = now.AddDays(-5),
                    DataPrevistaInicioServico = now.AddDays(-3),
                    DataFimServico = now.AddDays(-2),
                    Servico = "Troca de óleo",
                    Observacoes = "Trocar óleo e filtro.",
                    Estado = "Concluída",
                    ViaturaFK = viaturas.FirstOrDefault(v => v.Matricula == "33-IJ-44")?.ViaturaId ?? 0
                };
                marcacoes.Add(marc2);

                // Marcação 3 - Carlos, Focus
                var marc3 = new Marcacao
                {
                    DataMarcacaoFeita = now.AddDays(-2),
                    DataPrevistaInicioServico = now.AddDays(1),
                    DataFimServico = null,
                    Servico = "Alinhar direção",
                    Observacoes = "Cliente refere volante desalinhado.",
                    Estado = "Agendada",
                    ViaturaFK = viaturas.FirstOrDefault(v => v.Matricula == "55-KL-66")?.ViaturaId ?? 0
                };
                marcacoes.Add(marc3);

                // Marcação 4 - João, Golf
                var marc4 = new Marcacao
                {
                    DataMarcacaoFeita = now.AddDays(-1),
                    DataPrevistaInicioServico = now,
                    DataFimServico = null,
                    Servico = "Substituição de pastilhas de travão",
                    Observacoes = "Aviso de desgaste aceso.",
                    Estado = "Em Progresso",
                    ViaturaFK = viaturas.FirstOrDefault(v => v.Matricula == "78-EF-90")?.ViaturaId ?? 0
                };
                marcacoes.Add(marc4);

                context.Marcacoes.AddRange(marcacoes);
                context.SaveChanges();

                // Associar funcionários às marcações
                var func1 = funcionarios.FirstOrDefault(f => f.Email == "maria@email.com");
                var func2 = funcionarios.FirstOrDefault(f => f.Email == "rui@email.com");
                var func3 = funcionarios.FirstOrDefault(f => f.Email == "ines@email.com");

                if (func1 != null && marc1.MarcacaoId != 0)
                {
                    funcionariosMarcacoes.Add(new FuncionariosMarcacoes
                    {
                        MarcacaoFK = marc1.MarcacaoId,
                        FuncionarioFK = func1.UtilizadorId,
                        HorasGastas = 3.5m,
                        Comentarios = "Revisão feita sem problemas.",
                        DataInicioServico = marc1.DataPrevistaInicioServico
                    });
                }
                if (func2 != null && marc2.MarcacaoId != 0)
                {
                    funcionariosMarcacoes.Add(new FuncionariosMarcacoes
                    {
                        MarcacaoFK = marc2.MarcacaoId,
                        FuncionarioFK = func2.UtilizadorId,
                        HorasGastas = 1.2m,
                        Comentarios = "Óleo e filtro substituídos.",
                        DataInicioServico = marc2.DataPrevistaInicioServico
                    });
                }
                if (func3 != null && marc3.MarcacaoId != 0)
                {
                    funcionariosMarcacoes.Add(new FuncionariosMarcacoes
                    {
                        MarcacaoFK = marc3.MarcacaoId,
                        FuncionarioFK = func3.UtilizadorId,
                        HorasGastas = 0,
                        Comentarios = "Agendada para breve.",
                        DataInicioServico = marc3.DataPrevistaInicioServico
                    });
                }
                if (func1 != null && marc4.MarcacaoId != 0)
                {
                    funcionariosMarcacoes.Add(new FuncionariosMarcacoes
                    {
                        MarcacaoFK = marc4.MarcacaoId,
                        FuncionarioFK = func1.UtilizadorId,
                        HorasGastas = 0.5m,
                        Comentarios = "Iniciado serviço de travões.",
                        DataInicioServico = marc4.DataPrevistaInicioServico
                    });
                }
                context.FuncionariosMarcacoes.AddRange(funcionariosMarcacoes);
                context.SaveChanges();
            }
        }
    }
}
