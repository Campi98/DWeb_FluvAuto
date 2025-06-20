using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluvAuto.Data;
using FluvAuto.Models;
using Microsoft.AspNetCore.Authorization;

namespace FluvAuto.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Gestor,Cliente")]     //TODO: AINDA NÃO SE FEZ NADA DE ROLES

    // [Authorize(AuthenticationSchemes ="Bearer")] // Auth por JWT
    // Supostamente ia buscar-se o IEnumerable com o ViaturasByUserDTO
    // falta passar os dados da pessoa autenticada aqui para dentro

    public class ViaturasAuthController : ControllerBase
    {
        private readonly ApplicationDbContext _bd;

        public ViaturasAuthController(ApplicationDbContext context)
        {
            _bd = context;
        }

        // GET: api/Viaturas
        /// <summary>
        /// Obtém a lista de todas as viaturas registadas na base de dados.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous] // Permite acesso anónimo para obter a lista de viaturas
        public async Task<ActionResult<IEnumerable<Viatura>>> GetViaturas()
        {
            return await _bd.Viaturas   // não entendi bem pq o prof fez isto aqui; será para enviar o cliente e marcações associadas?
                                    .Include(v => v.Cliente)
                                    .Include(v => v.Marcacoes)
                                    .ToListAsync();
        }

        // GET: api/Viaturas/5
        /// <summary>
        /// Obtém uma viatura específica com base no seu ID.
        /// </summary>
        /// <param name="id"> Identificador da viatura pretendida </param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Viatura>> GetViatura(int id)
        {
            var viatura = await _bd.Viaturas.FindAsync(id);

            if (viatura == null)
            {
                return NotFound();
            }

            return viatura;
        }

        // PUT: api/Viaturas/5
        /// <summary>
        /// Atualiza uma viatura existente na base de dados com base no seu ID.
        /// </summary>
        /// <param name="id"> Identificação da viatura a editar </param>
        /// <param name="viaturaAlterada"> Novos dados da viatura </param>
        /// <returns></returns>
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
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
        /// </summary>
        /// <param name="viaturaNova"> Dados da viatura a ser criada</param>
        /// <returns></returns>
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Viatura>> PostViatura(Viatura viaturaNova)
        {
            _bd.Viaturas.Add(viaturaNova);
            await _bd.SaveChangesAsync();

            return CreatedAtAction("GetViatura", new { id = viaturaNova.ViaturaId }, viaturaNova);
        }

        // DELETE: api/Viaturas/5
        /// <summary>
        /// Remove uma viatura da base de dados com base no seu ID.
        /// </summary>
        /// <param name="id"> Identificador da viatura a apagar </param>
        /// <returns></returns>
        [HttpDelete("{id}")]
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
