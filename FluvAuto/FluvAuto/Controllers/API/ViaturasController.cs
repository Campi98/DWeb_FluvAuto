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
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ViaturasController : ControllerBase
    {
        private readonly ApplicationDbContext _bd;

        public ViaturasController(ApplicationDbContext context)
        {
            _bd = context;
        }

        // GET: api/Viaturas
        /// <summary>
        /// Obtém viaturas baseado no papel do utilizador.
        /// Admin/Funcionario: todas as viaturas
        /// Cliente: apenas as suas viaturas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViaturaDTO>>> GetViaturas()
        {
            var isAdminOrFuncionario = User.IsInRole("admin") || User.IsInRole("funcionario");

            if (isAdminOrFuncionario)
            {
                var todasViaturas = await _bd.Viaturas
                    .Include(v => v.Cliente)
                    .Select(v => new ViaturaDTO
                    {
                        ViaturaId = v.ViaturaId,
                        Marca = v.Marca,
                        Modelo = v.Modelo,
                        Matricula = v.Matricula,
                        Ano = v.Ano,
                        Cor = v.Cor,
                        Combustivel = v.Combustivel,
                        Motorizacao = v.Motorizacao,
                        ClienteFK = v.ClienteFK,
                        ClienteNome = v.Cliente != null ? v.Cliente.Nome : null,
                        ClienteEmail = v.Cliente != null ? v.Cliente.Email : null,
                        ClienteTelefone = v.Cliente != null ? v.Cliente.Telefone : null,
                        NumeroMarcacoes = _bd.Marcacoes.Count(m => m.ViaturaFK == v.ViaturaId)
                    })
                    .ToListAsync();

                return todasViaturas;
            }

            // Cliente: só vê as suas viaturas
            var username = User.Identity?.Name;
            var clienteId = _bd.Clientes
                .Where(c => c.UserName == username)
                .Select(c => c.UtilizadorId)
                .FirstOrDefault();

            var viaturasCliente = await _bd.Viaturas
                .Include(v => v.Cliente)
                .Where(v => v.ClienteFK == clienteId)
                .Select(v => new ViaturaDTO
                {
                    ViaturaId = v.ViaturaId,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Matricula = v.Matricula,
                    Ano = v.Ano,
                    Cor = v.Cor,
                    Combustivel = v.Combustivel,
                    Motorizacao = v.Motorizacao,
                    ClienteFK = v.ClienteFK,
                    ClienteNome = v.Cliente != null ? v.Cliente.Nome : null,
                    ClienteEmail = v.Cliente != null ? v.Cliente.Email : null,
                    ClienteTelefone = v.Cliente != null ? v.Cliente.Telefone : null,
                    NumeroMarcacoes = _bd.Marcacoes.Count(m => m.ViaturaFK == v.ViaturaId)
                })
                .ToListAsync();

            return viaturasCliente;
        }

        // GET: api/Viaturas/5
        /// <summary>
        /// Obtém uma viatura específica com base no seu ID.
        /// Verifica se o utilizador tem permissão para aceder à viatura.
        /// </summary>
        /// <param name="id"> Identificador da viatura pretendida </param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ViaturaDTO>> GetViatura(int id)
        {
            var viatura = await _bd.Viaturas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.ViaturaId == id);

            if (viatura == null)
            {
                return NotFound();
            }

            // Verificar se o utilizador pode aceder a esta viatura
            if (!(User.IsInRole("admin") || User.IsInRole("funcionario")))
            {
                var username = User.Identity?.Name;
                var clienteId = _bd.Clientes
                    .Where(c => c.UserName == username)
                    .Select(c => c.UtilizadorId)
                    .FirstOrDefault();

                if (viatura.ClienteFK != clienteId)
                {
                    return Forbid();
                }
            }

            // Converter para DTO evitando referências circulares
            var viaturaDTO = new ViaturaDTO
            {
                ViaturaId = viatura.ViaturaId,
                Marca = viatura.Marca,
                Modelo = viatura.Modelo,
                Matricula = viatura.Matricula,
                Ano = viatura.Ano,
                Cor = viatura.Cor,
                Combustivel = viatura.Combustivel,
                Motorizacao = viatura.Motorizacao,
                ClienteFK = viatura.ClienteFK,
                ClienteNome = viatura.Cliente?.Nome,
                ClienteEmail = viatura.Cliente?.Email,
                ClienteTelefone = viatura.Cliente?.Telefone,
                NumeroMarcacoes = await _bd.Marcacoes.CountAsync(m => m.ViaturaFK == id)
            };

            return viaturaDTO;
        }

        // PUT: api/Viaturas/5
        /// <summary>
        /// Atualiza uma viatura existente na base de dados com base no seu ID.
        /// Apenas admin e funcionarios podem editar viaturas.
        /// </summary>
        /// <param name="id"> Identificação da viatura a editar </param>
        /// <param name="viaturaAlterada"> Novos dados da viatura </param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin,funcionario")]
        public async Task<IActionResult> PutViatura(int id, Viatura viaturaAlterada)
        {
            if (id != viaturaAlterada.ViaturaId)
            {
                return BadRequest();
            }

            _bd.Entry(viaturaAlterada).State = EntityState.Modified;

            try
            {
                await _bd.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ViaturaExists(id))
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

        // POST: api/Viaturas
        /// <summary>
        /// Cria uma nova viatura na base de dados.
        /// Clientes só podem criar viaturas para si próprios.
        /// Admin/Funcionarios podem criar para qualquer cliente ?? - ver
        /// </summary>
        /// <param name="viaturaNova"> Dados da viatura a ser criada</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Viatura>> PostViatura(Viatura viaturaNova)
        {
            // Se não é admin/funcionario, só pode criar viaturas para si próprio
            if (!(User.IsInRole("admin") || User.IsInRole("funcionario")))
            {
                var username = User.Identity?.Name;
                var clienteId = _bd.Clientes
                    .Where(c => c.UserName == username)
                    .Select(c => c.UtilizadorId)
                    .FirstOrDefault();

                viaturaNova.ClienteFK = clienteId;
            }

            _bd.Viaturas.Add(viaturaNova);
            await _bd.SaveChangesAsync();

            return CreatedAtAction("GetViatura", new { id = viaturaNova.ViaturaId }, viaturaNova);
        }

        // DELETE: api/Viaturas/5
        /// <summary>
        /// Remove uma viatura da base de dados com base no seu ID.
        /// Apenas admin e funcionarios podem eliminar viaturas.
        /// </summary>
        /// <param name="id"> Identificador da viatura a apagar </param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin,funcionario")]
        public async Task<IActionResult> DeleteViatura(int id)
        {
            var viatura = await _bd.Viaturas.FindAsync(id);
            if (viatura == null)
            {
                return NotFound();
            }

            _bd.Viaturas.Remove(viatura);
            await _bd.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica se uma viatura com o ID especificado já existe na base de dados.
        /// </summary>
        /// <param name="id"> Identificador da viatura a procurar </param>
        /// <returns> 'true', se a viatura existe, senão 'false' </returns>
        private bool ViaturaExists(int id)
        {
            return _bd.Viaturas.Any(e => e.ViaturaId == id);
        }
    }
}
