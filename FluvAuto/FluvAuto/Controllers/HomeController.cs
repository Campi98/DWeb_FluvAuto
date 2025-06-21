using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FluvAuto.Models;
using FluvAuto.Data;
using Microsoft.EntityFrameworkCore;

namespace FluvAuto.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View("Privacy");
    }

    public IActionResult UserPage()
    {
        var username = User.Identity?.Name;
        var clienteId = _context.Clientes.Where(c => c.UserName == username).Select(c => c.UtilizadorId).FirstOrDefault();
        var datasMarcacoes = _context.Marcacoes
            .Where(m => m.Viatura != null && m.Viatura.ClienteFK == clienteId)
            .Include(m => m.Viatura)
            .Select(m => m.DataMarcacaoFeita.Date)
            .ToList();
        var datasMarcacoesStr = datasMarcacoes.Select(d => d.ToString("yyyy-MM-dd")).ToList();
        ViewBag.DatasMarcacoes = datasMarcacoesStr;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
