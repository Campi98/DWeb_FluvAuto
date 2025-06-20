using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluvAuto.Data;
using FluvAuto.Models;

namespace FluvAuto.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViaturasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ViaturasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Viaturas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Viatura>>> GetViaturas()
        {
            return await _context.Viaturas.ToListAsync();
        }

        // GET: api/Viaturas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Viatura>> GetViatura(int id)
        {
            var viatura = await _context.Viaturas.FindAsync(id);

            if (viatura == null)
            {
                return NotFound();
            }

            return viatura;
        }

        // PUT: api/Viaturas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutViatura(int id, Viatura viatura)
        {
            if (id != viatura.ViaturaId)
            {
                return BadRequest();
            }

            _context.Entry(viatura).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Viatura>> PostViatura(Viatura viatura)
        {
            _context.Viaturas.Add(viatura);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetViatura", new { id = viatura.ViaturaId }, viatura);
        }

        // DELETE: api/Viaturas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteViatura(int id)
        {
            var viatura = await _context.Viaturas.FindAsync(id);
            if (viatura == null)
            {
                return NotFound();
            }

            _context.Viaturas.Remove(viatura);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ViaturaExists(int id)
        {
            return _context.Viaturas.Any(e => e.ViaturaId == id);
        }
    }
}
