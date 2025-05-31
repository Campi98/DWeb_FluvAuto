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
        private readonly ApplicationDbContext _bd;

        public ViaturasController(ApplicationDbContext context)
        {
            _bd = context;
        }

        // GET: Viaturas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _bd.Viaturas.Include(v => v.Cliente);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Viaturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viatura = await _bd.Viaturas
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
            ViewData["ClienteFK"] = new SelectList(_bd.Clientes, "UtilizadorId", "Email");
            return View();
        }

        // POST: Viaturas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ViaturaId,Marca,Modelo,Matricula,Ano,Cor,Combustivel,Motorizacao,ClienteFK")] Viatura viaturaNova)
        {
            if (ModelState.IsValid)
            {
                _bd.Add(viaturaNova);
                await _bd.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteFK"] = new SelectList(_bd.Clientes, "UtilizadorId", "Email", viaturaNova.ClienteFK);
            return View(viaturaNova);
        }

        // GET: Viaturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viatura = await _bd.Viaturas.FindAsync(id);
            if (viatura == null)
            {
                return NotFound();
            }

            // se o código chega aqui, é porque há "viatura" para editar
            // temos que guardar os dados do objeto que vai ser enviado para o browser do utilizador
            HttpContext.Session.SetInt32("ViaturaId", viatura.ViaturaId);
            HttpContext.Session.SetString("Acao", "Viaturas/Edit"); // para saber que estamos a editar uma viatura, evitando trafulhices

            ViewData["ClienteFK"] = new SelectList(_bd.Clientes, "UtilizadorId", "Email", viatura.ClienteFK);
            return View(viatura);
        }

        // POST: Viaturas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("ViaturaId,Marca,Modelo,Matricula,Ano,Cor,Combustivel,Motorizacao,ClienteFK")] Viatura viaturaAlterada)
        {
            // o FromRoute lê o id da URL, se houve alterações à rota, houve alterações indevidas
            // Verifica se o id da rota corresponde ao da viatura recebida
            if (id != viaturaAlterada.ViaturaId)
            {
                return RedirectToAction(nameof(Index));
            }

            // Verifica se o ID da viatura está guardado na sessão e se a ação é válida
            var viaturaIdSession = HttpContext.Session.GetInt32("ViaturaId");
            var acao = HttpContext.Session.GetString("Acao");
            if (viaturaIdSession == null || string.IsNullOrEmpty(acao))
            {
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue alterar a viatura. Tem de reiniciar o processo.");
                ViewData["ClienteFK"] = new SelectList(_bd.Clientes, "UtilizadorId", "Email", viaturaAlterada.ClienteFK);      //TODO: ver se esta linha está certa, ou se é necessária
                // supostamente o cliente já estaria selecionado, e não pode alterar o cliente - vem da autenticação
                return View(viaturaAlterada);
            }

            // Verifica se o ID da sessão corresponde ao da viatura recebida e se a ação é válida
            if (viaturaIdSession != viaturaAlterada.ViaturaId || acao != "Viaturas/Edit")
            {
                // O utilizador está a tentar alterar outro objeto diferente do que recebeu
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _bd.Update(viaturaAlterada); // atualiza o objeto viatura na base de dados
                    await _bd.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViaturaExists(viaturaAlterada.ViaturaId))
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
            ViewData["ClienteFK"] = new SelectList(_bd.Clientes, "UtilizadorId", "Email", viaturaAlterada.ClienteFK);
            return View(viaturaAlterada);
        }

        // GET: Viaturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viatura = await _bd.Viaturas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(m => m.ViaturaId == id);
            if (viatura == null)
            {
                return NotFound();
            }

            // Guardar os dados do objeto e a ação na sessão
            HttpContext.Session.SetInt32("ViaturaId", viatura.ViaturaId);
            HttpContext.Session.SetString("Acao", "Viaturas/Delete");

            return View(viatura);
        }

        // POST: Viaturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var viatura = await _bd.Viaturas.FindAsync(id);

            // Verifica se o ID da viatura está guardado na sessão e se a ação é válida
            var viaturaIdSession = HttpContext.Session.GetInt32("ViaturaId");
            var acao = HttpContext.Session.GetString("Acao");
            if (viaturaIdSession == null || string.IsNullOrEmpty(acao))
            {   // demorar muito tempo => timeout
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue eliminar a viatura. Tem de reiniciar o processo.");
                return View(viatura);
            }

            // se houve adulteração aos dados
            if (viaturaIdSession != (viatura?.ViaturaId ?? 0) || acao != "Viaturas/Delete")
            {
                return RedirectToAction(nameof(Index));
            }

            if (viatura != null)
            {
                _bd.Viaturas.Remove(viatura);
                await _bd.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ViaturaExists(int id)
        {
            return _bd.Viaturas.Any(e => e.ViaturaId == id);
        }
    }
}
