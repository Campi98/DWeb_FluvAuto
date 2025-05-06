using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FluvAuto.Data;
using FluvAuto.Models;


namespace FluvAuto.Controllers
{
    public class ViaturasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ViaturasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Viaturas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Viaturas.Include(v => v.Cliente);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Viaturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viatura = await _context.Viaturas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(m => m.ViaturaId == id);
            if (viatura == null)
            {
                return NotFound();
            }

            return View(viatura);
        }

        // GET: Viaturas/Create
        public IActionResult Create()
        {
            ViewData["ClienteFK"] = new SelectList(_context.Clientes, "ClienteId", "Email");
            return View();
        }

        // POST: Viaturas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ViaturaId,Marca,Modelo,Matricula,Ano,Cor,Combustivel,Motorizacao,ClienteFK")] Viatura viatura)
        {
            if (ModelState.IsValid)
            {
                _context.Add(viatura);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteFK"] = new SelectList(_context.Clientes, "ClienteId", "Email", viatura.ClienteFK);
            return View(viatura);
        }

        // GET: Viaturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viatura = await _context.Viaturas.FindAsync(id);
            if (viatura == null)
            {
                return NotFound();
            }
            ViewData["ClienteFK"] = new SelectList(_context.Clientes, "ClienteId", "Email", viatura.ClienteFK);
            return View(viatura);
        }

        // POST: Viaturas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ViaturaId,Marca,Modelo,Matricula,Ano,Cor,Combustivel,Motorizacao,ClienteFK")] Viatura viatura)
        {
            if (id != viatura.ViaturaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viatura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViaturaExists(viatura.ViaturaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteFK"] = new SelectList(_context.Clientes, "ClienteId", "Email", viatura.ClienteFK);
            return View(viatura);
        }

        // GET: Viaturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viatura = await _context.Viaturas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(m => m.ViaturaId == id);
            if (viatura == null)
            {
                return NotFound();
            }

            return View(viatura);
        }

        // POST: Viaturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var viatura = await _context.Viaturas.FindAsync(id);
            if (viatura != null)
            {
                _context.Viaturas.Remove(viatura);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ViaturaExists(int id)
        {
            return _context.Viaturas.Any(e => e.ViaturaId == id);
        }
    }
}
