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
    public class FuncionariosMarcacoesController : Controller
    {
        private readonly ApplicationDbContext _bd;

        public FuncionariosMarcacoesController(ApplicationDbContext context)
        {
            _bd = context;
        }

        // GET: FuncionariosMarcacoes
        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("admin") || User.IsInRole("funcionario"))
            {
                var applicationDbContext = _bd.FuncionariosMarcacoes
                    .Include(f => f.Funcionario)
                    .Include(f => f.Marcacao)
                        .ThenInclude(m => m.Viatura)
                            .ThenInclude(v => v.Cliente);
                return View(await applicationDbContext.ToListAsync());
            }
            // Cliente: só vê marcações das suas viaturas
            var username = User.Identity?.Name;
            var clienteId = _bd.Clientes.Where(c => c.UserName == username).Select(c => c.UtilizadorId).FirstOrDefault();
            var viaturasIds = _bd.Viaturas.Where(v => v.ClienteFK == clienteId).Select(v => v.ViaturaId).ToList();
            var marcacoesCliente = _bd.FuncionariosMarcacoes
                .Include(f => f.Funcionario)
                .Include(f => f.Marcacao)
                    .ThenInclude(m => m.Viatura)
                        .ThenInclude(v => v.Cliente)
                .Where(fm => viaturasIds.Contains(fm.Marcacao.ViaturaFK));
            return View(await marcacoesCliente.ToListAsync());
        }

        // GET: FuncionariosMarcacoes/Details?marcacaoId=1&funcionarioId=2
        [Authorize]
        public async Task<IActionResult> Details(int? marcacaoId, int? funcionarioId)
        {
            if (marcacaoId == null || funcionarioId == null)
            {
                return NotFound();
            }

            var funcionariosMarcacoes = await _bd.FuncionariosMarcacoes
                .Include(f => f.Funcionario)
                .Include(f => f.Marcacao)
                    .ThenInclude(m => m.Viatura)
                        .ThenInclude(v => v.Cliente)
                .FirstOrDefaultAsync(m => m.MarcacaoFK == marcacaoId && m.FuncionarioFK == funcionarioId);

            if (funcionariosMarcacoes == null)
            {
                return NotFound();
            }
            // Cliente: só pode ver se a viatura é sua
            if (!(User.IsInRole("admin") || User.IsInRole("funcionario")))
            {
                var username = User.Identity?.Name;
                var clienteId = _bd.Clientes.Where(c => c.UserName == username).Select(c => c.UtilizadorId).FirstOrDefault();
                if (funcionariosMarcacoes.Marcacao?.Viatura?.ClienteFK != clienteId)
                {
                    return Forbid();
                }
            }
            return View(funcionariosMarcacoes);
        }

        // GET: FuncionariosMarcacoes/Create
        [Authorize(Roles = "admin,funcionario")]
        public IActionResult Create()
        {
            ViewData["FuncionarioFK"] = new SelectList(_bd.Funcionarios, "UtilizadorId", "Nome");
            ViewData["MarcacaoFK"] = new SelectList(_bd.Marcacoes, "MarcacaoId", "Servico");
            return View();
        }

        // POST: FuncionariosMarcacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin,funcionario")]
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
            ViewData["FuncionarioFK"] = new SelectList(_bd.Funcionarios, "UtilizadorId", "Nome", funcionarioMarcacaoNova.FuncionarioFK);
            ViewData["MarcacaoFK"] = new SelectList(_bd.Marcacoes, "MarcacaoId", "Servico", funcionarioMarcacaoNova.MarcacaoFK);
            return View(funcionarioMarcacaoNova);
        }

        // GET: FuncionariosMarcacoes/Edit?marcacaoId=1&funcionarioId=2
        [Authorize(Roles = "admin,funcionario")]
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
            ViewData["FuncionarioFK"] = new SelectList(_bd.Funcionarios, "UtilizadorId", "Nome", funcionariosMarcacoes.FuncionarioFK);
            ViewData["MarcacaoFK"] = new SelectList(_bd.Marcacoes, "MarcacaoId", "Servico", funcionariosMarcacoes.MarcacaoFK);
            return View(funcionariosMarcacoes);
        }

        // POST: FuncionariosMarcacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin,funcionario")]
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
            ViewData["FuncionarioFK"] = new SelectList(_bd.Funcionarios, "UtilizadorId", "Nome", funcionarioMarcacaoAlterada.FuncionarioFK);
            ViewData["MarcacaoFK"] = new SelectList(_bd.Marcacoes, "MarcacaoId", "Servico", funcionarioMarcacaoAlterada.MarcacaoFK);
            return View(funcionarioMarcacaoAlterada);
        }

        // GET: FuncionariosMarcacoes/Delete/5
        [Authorize(Roles = "admin,funcionario")]
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
        [Authorize(Roles = "admin,funcionario")]
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
