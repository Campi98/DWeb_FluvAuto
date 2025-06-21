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
    public class MarcacoesController : Controller
    {
        private readonly ApplicationDbContext _bd;

        public MarcacoesController(ApplicationDbContext context)
        {
            _bd = context;
        }

        // GET: Marcacoes
        public async Task<IActionResult> Index()
        {
            var isAdminOrFuncionario = User.IsInRole("admin") || User.IsInRole("funcionario");
            if (isAdminOrFuncionario)
            {
                var applicationDbContext = _bd.Marcacoes
                    .Include(m => m.Viatura)
                    .ThenInclude(v => v.Cliente);
                return View(await applicationDbContext.ToListAsync());
            }
            // Cliente autenticado: só vê as suas próprias marcações
            var username = User.Identity?.Name;
            var clienteId = _bd.Clientes.Where(c => c.UserName == username).Select(c => c.UtilizadorId).FirstOrDefault();
            var marcacoesCliente = _bd.Marcacoes.Include(m => m.Viatura)
                .ThenInclude(v => v.Cliente)
                .Where(m => m.Viatura.ClienteFK == clienteId);
            ViewBag.ClienteId = clienteId;
            return View(await marcacoesCliente.ToListAsync());
        }

        // GET: Marcacoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marcacao = await _bd.Marcacoes
                .Include(m => m.Viatura)
                .FirstOrDefaultAsync(m => m.MarcacaoId == id);
            if (marcacao == null)
            {
                return NotFound();
            }

            return View(marcacao);
        }

        private List<SelectListItem> GetViaturasSelectList(int? selectedViaturaId = null, int? onlyClienteId = null)
        {
            List<Viatura> viaturas;
            if (onlyClienteId.HasValue)
            {
                viaturas = _bd.Viaturas.Where(v => v.ClienteFK == onlyClienteId.Value).ToList();
            }
            else
            {
                var isAdminOrFuncionario = User.IsInRole("admin") || User.IsInRole("funcionario");
                if (isAdminOrFuncionario)
                {
                    viaturas = _bd.Viaturas.ToList();
                }
                else
                {
                    var username = User.Identity?.Name;
                    var clienteId = _bd.Clientes.Where(c => c.UserName == username).Select(c => c.UtilizadorId).FirstOrDefault();
                    viaturas = _bd.Viaturas.Where(v => v.ClienteFK == clienteId).ToList();
                }
            }
            return viaturas.Select(v => new SelectListItem
            {
                Value = v.ViaturaId.ToString(),
                Text = $"{v.Marca} {v.Modelo} ({v.Matricula})",
                Selected = selectedViaturaId.HasValue && v.ViaturaId == selectedViaturaId.Value
            }).ToList();
        }

        // GET: Marcacoes/Create
        public IActionResult Create()
        {
            ViewData["ViaturaFK"] = GetViaturasSelectList();
            return View();
        }

        // POST: Marcacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MarcacaoId,DataMarcacaoFeita,DataPrevistaInicioServico,DataFimServico,Servico,Observacoes,Estado,ViaturaFK")] Marcacao marcacaoNova)
        {
            if (ModelState.IsValid)
            {
                _bd.Add(marcacaoNova);
                await _bd.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ViaturaFK"] = GetViaturasSelectList(marcacaoNova.ViaturaFK);
            return View(marcacaoNova);
        }

        [Authorize(Roles = "admin,funcionario")]
        // GET: Marcacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marcacao = await _bd.Marcacoes.Include(m => m.Viatura).FirstOrDefaultAsync(m => m.MarcacaoId == id);
            if (marcacao == null)
            {
                return NotFound();
            }

            // se o código chega aqui, é porque há "marcação" para editar
            // temos que guardar os dados do objeto que vai ser enviado para o browser do utilizador
            HttpContext.Session.SetInt32("MarcacaoId", marcacao.MarcacaoId);
            HttpContext.Session.SetString("Acao", "Marcacoes/Edit"); // para saber que estamos a editar uma marcação, evitando trafulhices

            int clienteId = marcacao.Viatura.ClienteFK;
            ViewData["ViaturaFK"] = GetViaturasSelectList(marcacao.ViaturaFK, clienteId);
            return View(marcacao);
        }

        [Authorize(Roles = "admin,funcionario")]
        // POST: Marcacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("MarcacaoId,DataMarcacaoFeita,DataPrevistaInicioServico,DataFimServico,Servico,Observacoes,Estado,ViaturaFK")] Marcacao marcacaoAlterada)
        {
            // o FromRoute lê o id da URL, se houve alterações à rota, houve alterações indevidas
            // Verifica se o id da rota corresponde ao da marcação recebida
            if (id != marcacaoAlterada.MarcacaoId)
            {
                return RedirectToAction(nameof(Index));
            }

            // Verifica se o ID da marcação está guardado na sessão e se a ação é válida
            var marcacaoIdSession = HttpContext.Session.GetInt32("MarcacaoId");
            var acao = HttpContext.Session.GetString("Acao");
            if (marcacaoIdSession == null || string.IsNullOrEmpty(acao))
            {
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue alterar a marcação. Tem de reiniciar o processo.");
                // Trazer tb o clienteId da viatura selecionada
                var viatura = _bd.Viaturas.FirstOrDefault(v => v.ViaturaId == marcacaoAlterada.ViaturaFK);
                int? clienteId = viatura?.ClienteFK;
                ViewData["ViaturaFK"] = GetViaturasSelectList(marcacaoAlterada.ViaturaFK, clienteId);
                return View(marcacaoAlterada);
            }

            // Verifica se o ID da sessão corresponde ao da marcação recebida e se a ação é válida
            if (marcacaoIdSession != marcacaoAlterada.MarcacaoId || acao != "Marcacoes/Edit")
            {
                // O utilizador está a tentar alterar outro objeto diferente do que recebeu
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _bd.Update(marcacaoAlterada);
                    await _bd.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarcacaoExists(marcacaoAlterada.MarcacaoId))
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
            // Trazer tb o clienteId da viatura selecionada
            var viatura2 = _bd.Viaturas.FirstOrDefault(v => v.ViaturaId == marcacaoAlterada.ViaturaFK);
            int? clienteId2 = viatura2?.ClienteFK;
            ViewData["ViaturaFK"] = GetViaturasSelectList(marcacaoAlterada.ViaturaFK, clienteId2);
            return View(marcacaoAlterada);
        }

        [Authorize(Roles = "admin,funcionario")]
        // GET: Marcacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marcacao = await _bd.Marcacoes
                .Include(m => m.Viatura)
                .FirstOrDefaultAsync(m => m.MarcacaoId == id);
            if (marcacao == null)
            {
                return NotFound();
            }

            // Guardar os dados do objeto e a ação na sessão
            HttpContext.Session.SetInt32("MarcacaoId", marcacao.MarcacaoId);
            HttpContext.Session.SetString("Acao", "Marcacoes/Delete");

            return View(marcacao);
        }

        [Authorize(Roles = "admin,funcionario")]
        // POST: Marcacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marcacao = await _bd.Marcacoes.FindAsync(id);

            // Verifica se o ID da marcação está guardado na sessão e se a ação é válida
            var marcacaoIdSession = HttpContext.Session.GetInt32("MarcacaoId");
            var acao = HttpContext.Session.GetString("Acao");
            if (marcacaoIdSession == null || string.IsNullOrEmpty(acao))
            {   // demorar muito tempo => timeout
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue eliminar a marcação. Tem de reiniciar o processo.");
                return View(marcacao);
            }

            // se houve adulteração aos dados
            if (marcacaoIdSession != (marcacao?.MarcacaoId ?? 0) || acao != "Marcacoes/Delete")
            {
                return RedirectToAction(nameof(Index));
            }

            if (marcacao != null)
            {
                _bd.Marcacoes.Remove(marcacao);
                await _bd.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MarcacaoExists(int id)
        {
            return _bd.Marcacoes.Any(e => e.MarcacaoId == id);
        }
    }
}
