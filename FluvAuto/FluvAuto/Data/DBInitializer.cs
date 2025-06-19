using FluvAuto.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace FluvAuto.Data
{
    public static class DBInitializer
    {
        public static void Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            // Garantir que a base de dados existe
            context.Database.Migrate();

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
        }
    }
}
