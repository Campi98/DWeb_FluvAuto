﻿using System;
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
    public class FuncionariosMarcacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FuncionariosMarcacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FuncionariosMarcacoes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DadosServicos.Include(f => f.Funcionario).Include(f => f.Marcacao);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FuncionariosMarcacoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionariosMarcacoes = await _context.DadosServicos
                .Include(f => f.Funcionario)
                .Include(f => f.Marcacao)
                .FirstOrDefaultAsync(m => m.MarcacaoFK == id);
            if (funcionariosMarcacoes == null)
            {
                return NotFound();
            }

            return View(funcionariosMarcacoes);
        }

        // GET: FuncionariosMarcacoes/Create
        public IActionResult Create()
        {
            ViewData["FuncionarioFK"] = new SelectList(_context.Funcionarios, "UtilizadorId", "Email");
            ViewData["MarcacaoFK"] = new SelectList(_context.Marcacoes, "MarcacaoId", "Servico");
            return View();
        }

        // POST: FuncionariosMarcacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HorasGastas,Comentarios,DataInicioServico,MarcacaoFK,FuncionarioFK")] FuncionariosMarcacoes funcionariosMarcacoes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(funcionariosMarcacoes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FuncionarioFK"] = new SelectList(_context.Funcionarios, "UtilizadorId", "Email", funcionariosMarcacoes.FuncionarioFK);
            ViewData["MarcacaoFK"] = new SelectList(_context.Marcacoes, "MarcacaoId", "Servico", funcionariosMarcacoes.MarcacaoFK);
            return View(funcionariosMarcacoes);
        }

        // GET: FuncionariosMarcacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionariosMarcacoes = await _context.DadosServicos.FindAsync(id);
            if (funcionariosMarcacoes == null)
            {
                return NotFound();
            }
            ViewData["FuncionarioFK"] = new SelectList(_context.Funcionarios, "UtilizadorId", "Email", funcionariosMarcacoes.FuncionarioFK);
            ViewData["MarcacaoFK"] = new SelectList(_context.Marcacoes, "MarcacaoId", "Servico", funcionariosMarcacoes.MarcacaoFK);
            return View(funcionariosMarcacoes);
        }

        // POST: FuncionariosMarcacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HorasGastas,Comentarios,DataInicioServico,MarcacaoFK,FuncionarioFK")] FuncionariosMarcacoes funcionariosMarcacoes)
        {
            if (id != funcionariosMarcacoes.MarcacaoFK)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funcionariosMarcacoes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FuncionariosMarcacoesExists(funcionariosMarcacoes.MarcacaoFK))
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
            ViewData["FuncionarioFK"] = new SelectList(_context.Funcionarios, "UtilizadorId", "Email", funcionariosMarcacoes.FuncionarioFK);
            ViewData["MarcacaoFK"] = new SelectList(_context.Marcacoes, "MarcacaoId", "Servico", funcionariosMarcacoes.MarcacaoFK);
            return View(funcionariosMarcacoes);
        }

        // GET: FuncionariosMarcacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionariosMarcacoes = await _context.DadosServicos
                .Include(f => f.Funcionario)
                .Include(f => f.Marcacao)
                .FirstOrDefaultAsync(m => m.MarcacaoFK == id);
            if (funcionariosMarcacoes == null)
            {
                return NotFound();
            }

            return View(funcionariosMarcacoes);
        }

        // POST: FuncionariosMarcacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var funcionariosMarcacoes = await _context.DadosServicos.FindAsync(id);
            if (funcionariosMarcacoes != null)
            {
                _context.DadosServicos.Remove(funcionariosMarcacoes);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FuncionariosMarcacoesExists(int id)
        {
            return _context.DadosServicos.Any(e => e.MarcacaoFK == id);
        }
    }
}
