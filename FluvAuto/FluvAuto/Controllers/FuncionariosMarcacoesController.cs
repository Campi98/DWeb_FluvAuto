using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FluvAuto.Data;
using FluvAuto.Models;
using Microsoft.AspNetCore.Authorization;

namespace FluvAuto.Controllers
{
    [Authorize]
    public class FuncionariosMarcacoesController : Controller
    {
        private readonly ApplicationDbContext _bd;

        public FuncionariosMarcacoesController(ApplicationDbContext context)
        {
            _bd = context;
        }

        // GET: FuncionariosMarcacoes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _bd.FuncionariosMarcacoes.Include(f => f.Funcionario).Include(f => f.Marcacao);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FuncionariosMarcacoes/Details?marcacaoId=1&funcionarioId=2
        public async Task<IActionResult> Details(int? marcacaoId, int? funcionarioId)
        {
            if (marcacaoId == null || funcionarioId == null)
            {
                return NotFound();
            }

            var funcionariosMarcacoes = await _bd.FuncionariosMarcacoes
                .Include(f => f.Funcionario)
                .Include(f => f.Marcacao)
                .FirstOrDefaultAsync(m => m.MarcacaoFK == marcacaoId && m.FuncionarioFK == funcionarioId);

            if (funcionariosMarcacoes == null)
            {
                return NotFound();
            }

            return View(funcionariosMarcacoes);
        }

        // GET: FuncionariosMarcacoes/Create
        public IActionResult Create()
        {
            ViewData["FuncionarioFK"] = new SelectList(_bd.Funcionarios, "UtilizadorId", "Email");
            ViewData["MarcacaoFK"] = new SelectList(_bd.Marcacoes, "MarcacaoId", "Servico");
            return View();
        }

        // POST: FuncionariosMarcacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HorasGastas,Comentarios,DataInicioServico,MarcacaoFK,FuncionarioFK")] FuncionariosMarcacoes funcionarioMarcacaoNova)
        {
            if (ModelState.IsValid)
            {
                _bd.Add(funcionarioMarcacaoNova);
                await _bd.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FuncionarioFK"] = new SelectList(_bd.Funcionarios, "UtilizadorId", "Email", funcionarioMarcacaoNova.FuncionarioFK);
            ViewData["MarcacaoFK"] = new SelectList(_bd.Marcacoes, "MarcacaoId", "Servico", funcionarioMarcacaoNova.MarcacaoFK);
            return View(funcionarioMarcacaoNova);
        }

        // GET: FuncionariosMarcacoes/Edit?marcacaoId=1&funcionarioId=2
        public async Task<IActionResult> Edit(int? marcacaoId, int? funcionarioId)
        {
            if (marcacaoId == null || funcionarioId == null)
            {
                return NotFound();
            }

            var funcionariosMarcacoes = await _bd.FuncionariosMarcacoes
                .FirstOrDefaultAsync(m => m.MarcacaoFK == marcacaoId && m.FuncionarioFK == funcionarioId);

            if (funcionariosMarcacoes == null)
            {
                return NotFound();
            }
            ViewData["FuncionarioFK"] = new SelectList(_bd.Funcionarios, "UtilizadorId", "Email", funcionariosMarcacoes.FuncionarioFK);
            ViewData["MarcacaoFK"] = new SelectList(_bd.Marcacoes, "MarcacaoId", "Servico", funcionariosMarcacoes.MarcacaoFK);
            return View(funcionariosMarcacoes);
        }

        // POST: FuncionariosMarcacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int marcacaoId, int funcionarioId, [Bind("HorasGastas,Comentarios,DataInicioServico,MarcacaoFK,FuncionarioFK")] FuncionariosMarcacoes funcionarioMarcacaoAlterada)
        {
            if (marcacaoId != funcionarioMarcacaoAlterada.MarcacaoFK || funcionarioId != funcionarioMarcacaoAlterada.FuncionarioFK)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _bd.Update(funcionarioMarcacaoAlterada);
                    await _bd.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FuncionariosMarcacoesExists(marcacaoId, funcionarioId))
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
            ViewData["FuncionarioFK"] = new SelectList(_bd.Funcionarios, "UtilizadorId", "Email", funcionarioMarcacaoAlterada.FuncionarioFK);
            ViewData["MarcacaoFK"] = new SelectList(_bd.Marcacoes, "MarcacaoId", "Servico", funcionarioMarcacaoAlterada.MarcacaoFK);
            return View(funcionarioMarcacaoAlterada);
        }

        // GET: FuncionariosMarcacoes/Delete/5
        public async Task<IActionResult> Delete(int? marcacaoId, int? funcionarioId)
        {
            if (marcacaoId == null || funcionarioId == null)
            {
                return NotFound();
            }

            var funcionariosMarcacoes = await _bd.FuncionariosMarcacoes
                .Include(f => f.Funcionario)
                .Include(f => f.Marcacao)
                .FirstOrDefaultAsync(m => m.MarcacaoFK == marcacaoId && m.FuncionarioFK == funcionarioId);

            if (funcionariosMarcacoes == null)
            {
                return NotFound();
            }

            return View(funcionariosMarcacoes);
        }

        // POST: FuncionariosMarcacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int marcacaoId, int funcionarioId)
        {
            var funcionariosMarcacoes = await _bd.FuncionariosMarcacoes
                .FirstOrDefaultAsync(m => m.MarcacaoFK == marcacaoId && m.FuncionarioFK == funcionarioId);

            if (funcionariosMarcacoes != null)
            {
                _bd.FuncionariosMarcacoes.Remove(funcionariosMarcacoes);
                await _bd.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FuncionariosMarcacoesExists(int marcacaoId, int funcionarioId)
        {
            return _bd.FuncionariosMarcacoes.Any(e => e.MarcacaoFK == marcacaoId && e.FuncionarioFK == funcionarioId);
        }
    }
}
