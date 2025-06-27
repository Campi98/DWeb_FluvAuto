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
    [Authorize(Roles = "admin")]
    public class FuncionariosController : Controller
    {
        private readonly ApplicationDbContext _bd;

        public FuncionariosController(ApplicationDbContext context)
        {
            _bd = context;
        }

        // GET: Funcionarios
        public async Task<IActionResult> Index()
        {
            return View(await _bd.Funcionarios.ToListAsync());
        }

        // GET: Funcionarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _bd.Funcionarios
                .FirstOrDefaultAsync(m => m.UtilizadorId == id);
            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);
        }

        // GET: Funcionarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Funcionarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Funcao,UtilizadorId,UserName,Nome,Email,Telefone,Morada,CodPostal")] Funcionario funcionarioNovo, IFormFile? fotografiaUpload)
        {
            // Processar upload da fotografia se existir
            if (fotografiaUpload != null && fotografiaUpload.Length > 0)
            {
                var caminhoFotografia = await ProcessarUploadFotografia(fotografiaUpload);
                if (!string.IsNullOrEmpty(caminhoFotografia))
                {
                    funcionarioNovo.Fotografia = caminhoFotografia;
                }
            }

            if (ModelState.IsValid)
            {
                _bd.Add(funcionarioNovo);
                await _bd.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(funcionarioNovo);
        }

        // GET: Funcionarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _bd.Funcionarios.FindAsync(id);
            if (funcionario == null)
            {
                return NotFound();
            }

            // se o código chega aqui, é porque há "funcionário" para editar
            // temos que guardar os dados do objeto que vai ser enviado para o browser do utilizador
            HttpContext.Session.SetInt32("FuncionarioId", funcionario.UtilizadorId);
            HttpContext.Session.SetString("Acao", "Funcionarios/Edit"); // para saber que estamos a editar um funcionário, evitando trafulhices

            return View(funcionario);
        }

        // POST: Funcionarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("Funcao,UtilizadorId,UserName,Nome,Email,Telefone,Morada,CodPostal")] Funcionario funcionarioAlterado, IFormFile? fotografiaUpload)
        {
            // o FromRoute lê o id da URL, se houve alterações à rota, houve alterações indevidas
            // Verifica se o id da rota corresponde ao do funcionário recebido
            if (id != funcionarioAlterado.UtilizadorId)
            {
                return RedirectToAction(nameof(Index));
            }

            // Verifica se o ID do funcionário está guardado na sessão e se a ação é válida
            var funcionarioIdSession = HttpContext.Session.GetInt32("FuncionarioId");
            var acao = HttpContext.Session.GetString("Acao");
            if (funcionarioIdSession == null || string.IsNullOrEmpty(acao))
            {
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue alterar o funcionário. Tem de reiniciar o processo.");
                return View(funcionarioAlterado);
            }

            // Verifica se o ID da sessão corresponde ao do funcionário recebido e se a ação é válida
            if (funcionarioIdSession != funcionarioAlterado.UtilizadorId || acao != "Funcionarios/Edit")
            {
                // O utilizador está a tentar alterar outro objeto diferente do que recebeu
                return RedirectToAction(nameof(Index));
            }

            // Processar upload da fotografia se existir
            if (fotografiaUpload != null && fotografiaUpload.Length > 0)
            {
                var caminhoFotografia = await ProcessarUploadFotografia(fotografiaUpload);
                if (!string.IsNullOrEmpty(caminhoFotografia))
                {
                    funcionarioAlterado.Fotografia = caminhoFotografia;
                }
            }
            else
            {
                // Manter a fotografia existente se não houver upload novo
                var funcionarioExistente = await _bd.Funcionarios.AsNoTracking().FirstOrDefaultAsync(f => f.UtilizadorId == funcionarioAlterado.UtilizadorId);
                if (funcionarioExistente != null)
                {
                    funcionarioAlterado.Fotografia = funcionarioExistente.Fotografia;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _bd.Update(funcionarioAlterado);
                    await _bd.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FuncionarioExists(funcionarioAlterado.UtilizadorId))
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
            return View(funcionarioAlterado);
        }

        // GET: Funcionarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _bd.Funcionarios
                .FirstOrDefaultAsync(m => m.UtilizadorId == id);
            if (funcionario == null)
            {
                return NotFound();
            }

            // Guardar os dados do objeto e a ação na sessão
            HttpContext.Session.SetInt32("FuncionarioId", funcionario.UtilizadorId);
            HttpContext.Session.SetString("Acao", "Funcionarios/Delete");

            return View(funcionario);
        }

        // POST: Funcionarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var funcionario = await _bd.Funcionarios.FindAsync(id);

            // Verifica se o ID do funcionário está guardado na sessão e se a ação é válida
            var funcionarioIdSession = HttpContext.Session.GetInt32("FuncionarioId");
            var acao = HttpContext.Session.GetString("Acao");
            if (funcionarioIdSession == null || string.IsNullOrEmpty(acao))
            {   // demorar muito tempo => timeout
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue eliminar o funcionário. Tem de reiniciar o processo.");
                return View(funcionario);
            }

            // se houve adulteração aos dados
            if (funcionarioIdSession != (funcionario?.UtilizadorId ?? 0) || acao != "Funcionarios/Delete")
            {
                return RedirectToAction(nameof(Index));
            }

            if (funcionario != null)
            {
                _bd.Funcionarios.Remove(funcionario);
                await _bd.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FuncionarioExists(int id)
        {
            return _bd.Funcionarios.Any(e => e.UtilizadorId == id);
        }

        /// <summary>
        /// Processa o upload da fotografia e converte para Base64
        /// </summary>
        /// <param name="arquivo">Arquivo de imagem enviado</param>
        /// <returns>String Base64 da imagem</returns>
        private async Task<string?> ProcessarUploadFotografia(IFormFile arquivo)
        {
            // Validar se é uma imagem
            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            
            if (!extensoesPermitidas.Contains(extensao))
            {
                return null;
            }

            // Validar tamanho do arquivo (máximo 500KB)
            if (arquivo.Length > 500 * 1024)
            {
                return null;
            }

            try
            {
                // Converter para Base64
                using var memoryStream = new MemoryStream();
                await arquivo.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                var base64String = Convert.ToBase64String(imageBytes);
                
                // Determinar o tipo MIME da imagem
                var mimeType = extensao switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png", 
                    ".gif" => "image/gif",
                    ".bmp" => "image/bmp",
                    ".webp" => "image/webp",
                    _ => "image/jpeg"
                };

                // Retornar no formato data URL
                return $"data:{mimeType};base64,{base64String}";
            }
            catch
            {
                return null;
            }
        }
    }
}
