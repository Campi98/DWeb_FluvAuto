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
    public class MarcacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarcacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Marcacoes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Marcacoes.Include(m => m.Viatura);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Marcacoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marcacao = await _context.Marcacoes
                .Include(m => m.Viatura)
                .FirstOrDefaultAsync(m => m.MarcacaoId == id);
            if (marcacao == null)
            {
                return NotFound();
            }

            return View(marcacao);
        }

        // GET: Marcacoes/Create
        public IActionResult Create()
        {
            ViewData["ViaturaFK"] = new SelectList(_context.Viaturas, "ViaturaId", "Combustivel");
            return View();
        }

        // POST: Marcacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MarcacaoId,DataMarcacaoFeita,DataPrevistaInicioServico,DataFimServico,Servico,Observacoes,Estado,ViaturaFK")] Marcacao marcacao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(marcacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ViaturaFK"] = new SelectList(_context.Viaturas, "ViaturaId", "Combustivel", marcacao.ViaturaFK);
            return View(marcacao);
        }

        // GET: Marcacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marcacao = await _context.Marcacoes.FindAsync(id);
            if (marcacao == null)
            {
                return NotFound();
            }
            ViewData["ViaturaFK"] = new SelectList(_context.Viaturas, "ViaturaId", "Combustivel", marcacao.ViaturaFK);
            return View(marcacao);
        }

        // POST: Marcacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MarcacaoId,DataMarcacaoFeita,DataPrevistaInicioServico,DataFimServico,Servico,Observacoes,Estado,ViaturaFK")] Marcacao marcacao)
        {
            if (id != marcacao.MarcacaoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(marcacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarcacaoExists(marcacao.MarcacaoId))
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
            ViewData["ViaturaFK"] = new SelectList(_context.Viaturas, "ViaturaId", "Combustivel", marcacao.ViaturaFK);
            return View(marcacao);
        }

        // GET: Marcacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marcacao = await _context.Marcacoes
                .Include(m => m.Viatura)
                .FirstOrDefaultAsync(m => m.MarcacaoId == id);
            if (marcacao == null)
            {
                return NotFound();
            }

            return View(marcacao);
        }

        // POST: Marcacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marcacao = await _context.Marcacoes.FindAsync(id);
            if (marcacao != null)
            {
                _context.Marcacoes.Remove(marcacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarcacaoExists(int id)
        {
            return _context.Marcacoes.Any(e => e.MarcacaoId == id);
        }
    }
}
