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
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _bd;

        public ClientesController(ApplicationDbContext context)
        {
            _bd = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            return View(await _bd.Clientes.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _bd.Clientes
                .FirstOrDefaultAsync(m => m.UtilizadorId == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NIF,UtilizadorId,UserName,Nome,Email,Telefone,Morada,CodPostal")] Cliente clienteNovo)
        {
            if (ModelState.IsValid)
            {
                _bd.Add(clienteNovo);
                await _bd.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clienteNovo);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _bd.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            // se o código chega aqui, é porque há "cliente" para editar
            // temos que guardar os dados do objeto que vai ser enviado para o browser do utilizador
            HttpContext.Session.SetInt32("ClienteId", cliente.UtilizadorId);
            HttpContext.Session.SetString("Acao", "Clientes/Edit"); // para saber que estamos a editar um cliente, evitando trafulhices

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("NIF,UtilizadorId,UserName,Nome,Email,Telefone,Morada,CodPostal")] Cliente clienteAlterado)
        {
            // o FromRoute lê o id da URL, se houve alterações à rota, houve alterações indevidas
            // Verifica se o id da rota corresponde ao do cliente recebido
            if (id != clienteAlterado.UtilizadorId)
            {
                return RedirectToAction(nameof(Index));
            }

            // Verifica se o ID do cliente está guardado na sessão e se a ação é válida
            var clienteIdSession = HttpContext.Session.GetInt32("ClienteId");
            var acao = HttpContext.Session.GetString("Acao");
            if (clienteIdSession == null || string.IsNullOrEmpty(acao))
            {
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue alterar o cliente. Tem de reiniciar o processo.");
                return View(clienteAlterado);
            }

            // Verifica se o ID da sessão corresponde ao do cliente recebido e se a ação é válida
            if (clienteIdSession != clienteAlterado.UtilizadorId || acao != "Clientes/Edit")
            {
                // O utilizador está a tentar alterar outro objeto diferente do que recebeu
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _bd.Update(clienteAlterado);
                    await _bd.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(clienteAlterado.UtilizadorId))
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
            return View(clienteAlterado);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _bd.Clientes
                .FirstOrDefaultAsync(m => m.UtilizadorId == id);
            if (cliente == null)
            {
                return NotFound();
            }

            // Guardar os dados do objeto e a ação na sessão
            HttpContext.Session.SetInt32("ClienteId", cliente.UtilizadorId);
            HttpContext.Session.SetString("Acao", "Clientes/Delete");

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _bd.Clientes.FindAsync(id);

            // Verifica se o ID do cliente está guardado na sessão e se a ação é válida
            var clienteIdSession = HttpContext.Session.GetInt32("ClienteId");
            var acao = HttpContext.Session.GetString("Acao");
            if (clienteIdSession == null || string.IsNullOrEmpty(acao))
            {   // demorar muito tempo => timeout
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue eliminar o cliente. Tem de reiniciar o processo.");
                return View(cliente);
            }

            // se houve adulteração aos dados
            if (clienteIdSession != (cliente?.UtilizadorId ?? 0) || acao != "Clientes/Delete")
            {
                return RedirectToAction(nameof(Index));
            }

            if (cliente != null)
            {
                _bd.Clientes.Remove(cliente);
                await _bd.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _bd.Clientes.Any(e => e.UtilizadorId == id);
        }
    }
}
