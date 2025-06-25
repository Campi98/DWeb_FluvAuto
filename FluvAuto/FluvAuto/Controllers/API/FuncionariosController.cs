using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluvAuto.Data;
using FluvAuto.Models;
using FluvAuto.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace FluvAuto.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
    public class FuncionariosController : ControllerBase
    {
        private readonly ApplicationDbContext _bd;

        public FuncionariosController(ApplicationDbContext context)
        {
            _bd = context;
        }

        // GET: api/Funcionarios
        /// <summary>
        /// Obtém a lista de todos os funcionários
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FuncionarioDTO>>> GetFuncionarios()
        {
            var funcionarios = await _bd.Funcionarios
                .Select(f => new FuncionarioDTO
                {
                    UtilizadorId = f.UtilizadorId,
                    UserName = f.UserName,
                    Nome = f.Nome,
                    Email = f.Email,
                    Telefone = f.Telefone,
                    Morada = f.Morada,
                    CodPostal = f.CodPostal,
                    Funcao = f.Funcao,
                    Fotografia = f.Fotografia,
                    NumeroServicosRealizados = _bd.FuncionariosMarcacoes.Count(fm => fm.FuncionarioFK == f.UtilizadorId)
                })
                .ToListAsync();
                
            return funcionarios;
        }

        // GET: api/Funcionarios/5
        /// <summary>
        /// Obtém um funcionário específico com base no seu ID
        /// </summary>
        /// <param name="id">Identificador do funcionário</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<FuncionarioDTO>> GetFuncionario(int id)
        {
            var funcionario = await _bd.Funcionarios
                .FirstOrDefaultAsync(f => f.UtilizadorId == id);

            if (funcionario == null)
            {
                return NotFound();
            }

            var funcionarioDTO = new FuncionarioDTO
            {
                UtilizadorId = funcionario.UtilizadorId,
                UserName = funcionario.UserName,
                Nome = funcionario.Nome,
                Email = funcionario.Email,
                Telefone = funcionario.Telefone,
                Morada = funcionario.Morada,
                CodPostal = funcionario.CodPostal,
                Funcao = funcionario.Funcao,
                Fotografia = funcionario.Fotografia,
                NumeroServicosRealizados = await _bd.FuncionariosMarcacoes.CountAsync(fm => fm.FuncionarioFK == id)
            };

            return funcionarioDTO;
        }

        // PUT: api/Funcionarios/5
        /// <summary>
        /// Atualiza um funcionário existente na base de dados com base no seu ID
        /// </summary>
        /// <param name="id">Identificação do funcionário a editar</param>
        /// <param name="funcionarioAlterado">Novos dados do funcionário</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFuncionario(int id, Funcionario funcionarioAlterado)
        {
            if (id != funcionarioAlterado.UtilizadorId)
            {
                return BadRequest();
            }

            _bd.Entry(funcionarioAlterado).State = EntityState.Modified;

            try
            {
                await _bd.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FuncionarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Funcionarios
        /// <summary>
        /// Cria um novo funcionário na base de dados
        /// </summary>
        /// <param name="funcionarioNovo">Dados do funcionário a ser criado</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Funcionario>> PostFuncionario(Funcionario funcionarioNovo)
        {
            _bd.Funcionarios.Add(funcionarioNovo);
            await _bd.SaveChangesAsync();

            return CreatedAtAction("GetFuncionario", new { id = funcionarioNovo.UtilizadorId }, funcionarioNovo);
        }

        // DELETE: api/Funcionarios/5
        /// <summary>
        /// Remove um funcionário da base de dados com base no seu ID
        /// </summary>
        /// <param name="id">Identificador do funcionário a apagar</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFuncionario(int id)
        {
            var funcionario = await _bd.Funcionarios.FindAsync(id);
            if (funcionario == null)
            {
                return NotFound();
            }

            _bd.Funcionarios.Remove(funcionario);
            await _bd.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica se um funcionário com o ID especificado já existe na base de dados
        /// </summary>
        /// <param name="id">Identificador do funcionário a procurar</param>
        /// <returns>'true', se o funcionário existe, senão 'false'</returns>
        private bool FuncionarioExists(int id)
        {
            return _bd.Funcionarios.Any(e => e.UtilizadorId == id);
        }
    }
}
