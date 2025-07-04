﻿using System;
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
    public class ViaturasController : Controller
    {
        private readonly ApplicationDbContext _bd;

        public ViaturasController(ApplicationDbContext context)
        {
            _bd = context;
        }

        // GET: Viaturas
        public async Task<IActionResult> Index(string searchString, string searchField)
        {
            if (User.IsInRole("admin") || User.IsInRole("funcionario"))
            {
                var todasViaturas = _bd.Viaturas.Include(v => v.Cliente);
                if (!string.IsNullOrEmpty(searchString))
                {
                    IQueryable<Viatura> filtradas = todasViaturas;
                    switch (searchField)
                    {
                        case "matricula":
                            filtradas = filtradas.Where(v => v.Matricula != null && v.Matricula.Contains(searchString));
                            break;
                        case "telefone":
                            filtradas = filtradas.Where(v => v.Cliente != null && v.Cliente.Telefone != null && v.Cliente.Telefone.Contains(searchString));
                            break;
                        default:
                            filtradas = filtradas.Where(v => v.Cliente != null && v.Cliente.Nome.Contains(searchString));
                            break;
                    }
                    return View(await filtradas.ToListAsync());
                }
                return View(await todasViaturas.ToListAsync());
            }
            // Cliente: só vê as suas viaturas
            var username = User.Identity?.Name;
            var clienteId = _bd.Clientes.Where(c => c.UserName == username).Select(c => c.UtilizadorId).FirstOrDefault();
            var viaturasCliente = _bd.Viaturas.Include(v => v.Cliente).Where(v => v.ClienteFK == clienteId);
            return View(await viaturasCliente.ToListAsync());
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
            //ViewData["ClienteFK"] = new SelectList(_bd.Clientes, "UtilizadorId", "Email");
            return View();
        }

        // POST: Viaturas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ViaturaId,Marca,Modelo,Matricula,Ano,Cor,Combustivel,Motorizacao")] Viatura viaturaNova)
        {
            // Obter o username do utilizador autenticado
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                // Se não houver utilizador autenticado, retorna erro
                ModelState.AddModelError(string.Empty, "Utilizador não autenticado.");
                return View(viaturaNova);
            }
            // Obter o ID do utilizador autenticado
            var clienteId = _bd.Clientes
                .Where(c => c.UserName == username)
                .Select(c => c.UtilizadorId)
                .FirstOrDefault();

            if (clienteId == 0)
            {
                ModelState.AddModelError(string.Empty, "Cliente não encontrado.");
                return View(viaturaNova);
            }

            // Associar o cliente autenticado à viatura
            viaturaNova.ClienteFK = clienteId;

            if (ModelState.IsValid)
            {
                _bd.Add(viaturaNova);
                await _bd.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["ClienteFK"] = new SelectList(_bd.Clientes, "UtilizadorId", "Email", viaturaNova.ClienteFK);
            return View(viaturaNova);
        }

        // GET: Viaturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viatura = await _bd.Viaturas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.ViaturaId == id);
            if (viatura == null)
            {
                return NotFound();
            }

            // Verificar permissões: admin/funcionário pode editar qualquer viatura, cliente só as suas
            if (!User.IsInRole("admin") && !User.IsInRole("funcionario"))
            {
                var username = User.Identity?.Name;
                var clienteId = _bd.Clientes.Where(c => c.UserName == username).Select(c => c.UtilizadorId).FirstOrDefault();
                
                if (viatura.ClienteFK != clienteId)
                {
                    return Forbid(); // Cliente não pode editar viatura de outro cliente
                }
            }

            // se o código chega aqui, é porque há "viatura" para editar
            // temos que guardar os dados do objeto que vai ser enviado para o browser do utilizador
            HttpContext.Session.SetInt32("ViaturaId", viatura.ViaturaId);
            HttpContext.Session.SetString("Acao", "Viaturas/Edit"); // para saber que estamos a editar uma viatura, evitando trafulhices

            // Passar informações do cliente para a view (para mostrar de quem é a viatura)
            if (viatura.Cliente != null)
            {
                ViewBag.ClienteNome = viatura.Cliente.Nome;
                ViewBag.ClienteEmail = viatura.Cliente.Email;
            }

            return View(viatura);
        }

        // POST: Viaturas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("ViaturaId,Marca,Modelo,Matricula,Ano,Cor,Combustivel,Motorizacao")] Viatura viaturaAlterada)
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
                //ViewData["ClienteFK"] = new SelectList(_bd.Clientes, "UtilizadorId", "Email", viaturaAlterada.ClienteFK);      //TODO: ver se esta linha está certa, ou se é necessária
                // supostamente o cliente já estaria selecionado, e não pode alterar o cliente - vem da autenticação
                return View(viaturaAlterada);
            }

            // Verifica se o ID da sessão corresponde ao da viatura recebida e se a ação é válida
            if (viaturaIdSession != viaturaAlterada.ViaturaId || acao != "Viaturas/Edit")
            {
                // O utilizador está a tentar alterar outro objeto diferente do que recebeu
                return RedirectToAction(nameof(Index));
            }

            // Obter o ClienteFK original da viatura (para manter o proprietário original)
            var clienteFKOriginal = await _bd.Viaturas
                .Where(v => v.ViaturaId == viaturaAlterada.ViaturaId)
                .Select(v => v.ClienteFK)
                .FirstOrDefaultAsync();

            if (clienteFKOriginal == 0)
            {
                return NotFound();
            }

            // Se for admin ou funcionário, manter o ClienteFK original
            // Se for cliente, verificar se é o proprietário da viatura
            if (User.IsInRole("admin") || User.IsInRole("funcionario"))
            {
                // Admin/Funcionário pode editar qualquer viatura, mantendo o proprietário original
                viaturaAlterada.ClienteFK = clienteFKOriginal;
            }
            else
            {
                // Cliente: verificar se é o proprietário da viatura
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    ModelState.AddModelError(string.Empty, "Utilizador não autenticado.");
                    return View(viaturaAlterada);
                }
            // Obter o ID do utilizador autenticado
                var clienteId = _bd.Clientes
                    .Where(c => c.UserName == username)
                    .Select(c => c.UtilizadorId)
                    .FirstOrDefault();

                if (clienteId == 0)
                {
                    ModelState.AddModelError(string.Empty, "Cliente não encontrado.");
                    return View(viaturaAlterada);
                }

                // Verificar se o cliente é o proprietário da viatura
                if (clienteFKOriginal != clienteId)
                {
                    ModelState.AddModelError(string.Empty, "Não tem permissão para editar esta viatura.");
                    return View(viaturaAlterada);
                }

                // Manter o ClienteFK original (que é o mesmo do cliente autenticado)
                viaturaAlterada.ClienteFK = clienteId;
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
            //ViewData["ClienteFK"] = new SelectList(_bd.Clientes, "UtilizadorId", "Email", viaturaAlterada.ClienteFK);
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
