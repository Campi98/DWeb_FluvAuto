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
    [Authorize(Roles = "admin")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _bd;

        public ClientesController(ApplicationDbContext context)
        {
            _bd = context;
        }        
        
        // GET: api/Clientes
        /// <summary>
        /// Obtém a lista de todos os clientes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _bd.Clientes
                .Include(c => c.Viaturas)
                .ToListAsync();
        }

        // GET: api/Clientes/5
        /// <summary>
        /// Obtém um cliente específico com base no seu ID
        /// </summary>
        /// <param name="id">Identificador do cliente</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _bd.Clientes
                .Include(c => c.Viaturas)
                .FirstOrDefaultAsync(c => c.UtilizadorId == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }        
        
        // PUT: api/Clientes/5
        /// <summary>
        /// Atualiza um cliente existente 
        /// </summary>
        /// <param name="id">Identificador do cliente</param>
        /// <param name="clienteAlterado">Novos dados do cliente</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente clienteAlterado)
        {
            if (id != clienteAlterado.UtilizadorId)
            {
                return BadRequest();
            }

            _bd.Entry(clienteAlterado).State = EntityState.Modified;

            try
            {
                await _bd.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
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

        // POST: api/Clientes
        /// <summary>
        /// Cria um novo cliente -- isto é para ficar aqui?
        /// Clientes normais devem usar o registo normal da aplicação.
        /// </summary>
        /// <param name="clienteNovo">Dados do cliente a criar</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente clienteNovo)
        {
            _bd.Clientes.Add(clienteNovo);
            await _bd.SaveChangesAsync();

            return CreatedAtAction("GetCliente", new { id = clienteNovo.UtilizadorId }, clienteNovo);
        }

        // DELETE: api/Clientes/5
        /// <summary>
        /// Remove um cliente
        /// </summary>
        /// <param name="id">Identificador do cliente a remover</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _bd.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _bd.Clientes.Remove(cliente);
            await _bd.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica se um cliente com o ID especificado existe.
        /// </summary>
        /// <param name="id">Identificador do cliente</param>
        /// <returns>true se existe, false caso contrário</returns>
        private bool ClienteExists(int id)
        {
            return _bd.Clientes.Any(e => e.UtilizadorId == id);
        }
    }
}
