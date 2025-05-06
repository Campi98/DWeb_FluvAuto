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
    public class DadosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dados
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DadosServicos.Include(d => d.Funcionario).Include(d => d.Marcacao);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Dados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dados = await _context.DadosServicos
                .Include(d => d.Funcionario)
                .Include(d => d.Marcacao)
                .FirstOrDefaultAsync(m => m.DadosId == id);
            if (dados == null)
            {
                return NotFound();
            }

            return View(dados);
        }

        // GET: Dados/Create
        public IActionResult Create()
        {
            ViewData["FuncionarioFK"] = new SelectList(_context.Funcionarios, "FuncionarioId", "Email");
            ViewData["MarcacaoFK"] = new SelectList(_context.Marcacoes, "MarcacaoId", "Servico");
            return View();
        }

        // POST: Dados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DadosId,HorasGastas,Comentarios,MarcacaoFK,FuncionarioFK")] Dados dados)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dados);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FuncionarioFK"] = new SelectList(_context.Funcionarios, "FuncionarioId", "Email", dados.FuncionarioFK);
            ViewData["MarcacaoFK"] = new SelectList(_context.Marcacoes, "MarcacaoId", "Servico", dados.MarcacaoFK);
            return View(dados);
        }

        // GET: Dados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dados = await _context.DadosServicos.FindAsync(id);
            if (dados == null)
            {
                return NotFound();
            }
            ViewData["FuncionarioFK"] = new SelectList(_context.Funcionarios, "FuncionarioId", "Email", dados.FuncionarioFK);
            ViewData["MarcacaoFK"] = new SelectList(_context.Marcacoes, "MarcacaoId", "Servico", dados.MarcacaoFK);
            return View(dados);
        }

        // POST: Dados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DadosId,HorasGastas,Comentarios,MarcacaoFK,FuncionarioFK")] Dados dados)
        {
            if (id != dados.DadosId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dados);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DadosExists(dados.DadosId))
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
            ViewData["FuncionarioFK"] = new SelectList(_context.Funcionarios, "FuncionarioId", "Email", dados.FuncionarioFK);
            ViewData["MarcacaoFK"] = new SelectList(_context.Marcacoes, "MarcacaoId", "Servico", dados.MarcacaoFK);
            return View(dados);
        }

        // GET: Dados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dados = await _context.DadosServicos
                .Include(d => d.Funcionario)
                .Include(d => d.Marcacao)
                .FirstOrDefaultAsync(m => m.DadosId == id);
            if (dados == null)
            {
                return NotFound();
            }

            return View(dados);
        }

        // POST: Dados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dados = await _context.DadosServicos.FindAsync(id);
            if (dados != null)
            {
                _context.DadosServicos.Remove(dados);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DadosExists(int id)
        {
            return _context.DadosServicos.Any(e => e.DadosId == id);
        }
    }
}
