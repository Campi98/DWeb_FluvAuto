using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluvAuto.Data;
using FluvAuto.Models;
using FluvAuto.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace FluvAuto.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class FuncionariosMarcacoesController : ControllerBase
    {
        private readonly ApplicationDbContext _bd;

        public FuncionariosMarcacoesController(ApplicationDbContext context)
        {
            _bd = context;
        }

        // GET: api/FuncionariosMarcacoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicoDTO>>> GetFuncionariosMarcacoes()
        {
            var query = _bd.FuncionariosMarcacoes
                .Include(f => f.Funcionario)
                .Include(f => f.Marcacao)
                    .ThenInclude(m => m.Viatura)
                        .ThenInclude(v => v!.Cliente);

            if (User.IsInRole("admin") || User.IsInRole("funcionario"))
            {
                // Admin e funcionários veem todos os serviços
                var servicos = await query.ToListAsync();
                return Ok(servicos.Select(ServicoToDTO));
            }
            else
            {
                // Cliente: só vê serviços das suas viaturas
                var username = User.Identity?.Name;
                var clienteId = _bd.Clientes.Where(c => c.UserName == username).Select(c => c.UtilizadorId).FirstOrDefault();
                var viaturasIds = _bd.Viaturas.Where(v => v.ClienteFK == clienteId).Select(v => v.ViaturaId).ToList();
                
                var servicosCliente = await query
                    .Where(fm => fm.Marcacao != null && viaturasIds.Contains(fm.Marcacao.ViaturaFK))
                    .ToListAsync();
                
                return Ok(servicosCliente.Select(ServicoToDTO));
            }
        }

        // GET: api/FuncionariosMarcacoes/marcacaoId/funcionarioId
        [HttpGet("{marcacaoId}/{funcionarioId}")]
        public async Task<ActionResult<ServicoDTO>> GetFuncionariosMarcacoes(int marcacaoId, int funcionarioId)
        {
            var funcionariosMarcacoes = await _bd.FuncionariosMarcacoes
                .Include(f => f.Funcionario)
                .Include(f => f.Marcacao)
                    .ThenInclude(m => m.Viatura)
                        .ThenInclude(v => v!.Cliente)
                .FirstOrDefaultAsync(fm => fm.MarcacaoFK == marcacaoId && fm.FuncionarioFK == funcionarioId);

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

            return Ok(ServicoToDTO(funcionariosMarcacoes));
        }

        // PUT: api/FuncionariosMarcacoes/marcacaoId/funcionarioId
        [HttpPut("{marcacaoId}/{funcionarioId}")]
        [Authorize(Roles = "admin,funcionario")]
        public async Task<IActionResult> PutFuncionariosMarcacoes(int marcacaoId, int funcionarioId, ServicoDTO servicoDTO)
        {
            if (marcacaoId != servicoDTO.MarcacaoFK || funcionarioId != servicoDTO.FuncionarioFK)
            {
                return BadRequest();
            }

            var funcionariosMarcacoes = await _bd.FuncionariosMarcacoes
                .FirstOrDefaultAsync(fm => fm.MarcacaoFK == marcacaoId && fm.FuncionarioFK == funcionarioId);

            if (funcionariosMarcacoes == null)
            {
                return NotFound();
            }

            funcionariosMarcacoes.HorasGastas = servicoDTO.HorasGastas;
            funcionariosMarcacoes.Comentarios = servicoDTO.Comentarios;
            funcionariosMarcacoes.DataInicioServico = servicoDTO.DataInicioServico;

            _bd.Entry(funcionariosMarcacoes).State = EntityState.Modified;

            try
            {
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

            return NoContent();
        }

        // POST: api/FuncionariosMarcacoes
        [HttpPost]
        [Authorize(Roles = "admin,funcionario")]
        public async Task<ActionResult<ServicoDTO>> PostFuncionariosMarcacoes(ServicoDTO servicoDTO)
        {
            var funcionariosMarcacoes = new FuncionariosMarcacoes
            {
                MarcacaoFK = servicoDTO.MarcacaoFK,
                FuncionarioFK = servicoDTO.FuncionarioFK,
                HorasGastas = servicoDTO.HorasGastas,
                Comentarios = servicoDTO.Comentarios,
                DataInicioServico = servicoDTO.DataInicioServico
            };

            _bd.FuncionariosMarcacoes.Add(funcionariosMarcacoes);

            try
            {
                await _bd.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FuncionariosMarcacoesExists(servicoDTO.MarcacaoFK, servicoDTO.FuncionarioFK))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // Traz o objeto criado com as informações completas
            var servicoCriado = await _bd.FuncionariosMarcacoes
                .Include(f => f.Funcionario)
                .Include(f => f.Marcacao)
                    .ThenInclude(m => m.Viatura)
                        .ThenInclude(v => v!.Cliente)
                .FirstOrDefaultAsync(fm => fm.MarcacaoFK == servicoDTO.MarcacaoFK && fm.FuncionarioFK == servicoDTO.FuncionarioFK);

            return CreatedAtAction("GetFuncionariosMarcacoes", 
                new { marcacaoId = servicoDTO.MarcacaoFK, funcionarioId = servicoDTO.FuncionarioFK }, 
                ServicoToDTO(servicoCriado!));
        }

        // DELETE: api/FuncionariosMarcacoes/marcacaoId/funcionarioId
        [HttpDelete("{marcacaoId}/{funcionarioId}")]
        [Authorize(Roles = "admin,funcionario")]
        public async Task<IActionResult> DeleteFuncionariosMarcacoes(int marcacaoId, int funcionarioId)
        {
            var funcionariosMarcacoes = await _bd.FuncionariosMarcacoes
                .FirstOrDefaultAsync(fm => fm.MarcacaoFK == marcacaoId && fm.FuncionarioFK == funcionarioId);

            if (funcionariosMarcacoes == null)
            {
                return NotFound();
            }

            _bd.FuncionariosMarcacoes.Remove(funcionariosMarcacoes);
            await _bd.SaveChangesAsync();

            return NoContent();
        }

        private bool FuncionariosMarcacoesExists(int marcacaoId, int funcionarioId)
        {
            return _bd.FuncionariosMarcacoes.Any(e => e.MarcacaoFK == marcacaoId && e.FuncionarioFK == funcionarioId);
        }

        private static ServicoDTO ServicoToDTO(FuncionariosMarcacoes funcionariosMarcacoes)
        {
            return new ServicoDTO
            {
                MarcacaoFK = funcionariosMarcacoes.MarcacaoFK,
                FuncionarioFK = funcionariosMarcacoes.FuncionarioFK,
                HorasGastas = funcionariosMarcacoes.HorasGastas,
                Comentarios = funcionariosMarcacoes.Comentarios,
                DataInicioServico = funcionariosMarcacoes.DataInicioServico,
                // Dados da marcação
                MarcacaoServico = funcionariosMarcacoes.Marcacao?.Servico,
                MarcacaoDataPrevista = funcionariosMarcacoes.Marcacao?.DataPrevistaInicioServico,
                MarcacaoDataFim = funcionariosMarcacoes.Marcacao?.DataFimServico,
                MarcacaoEstado = funcionariosMarcacoes.Marcacao?.Estado,
                // Dados do funcionário
                FuncionarioNome = funcionariosMarcacoes.Funcionario?.Nome,
                FuncionarioFuncao = funcionariosMarcacoes.Funcionario?.Funcao,
                // Dados da viatura
                ViaturaMatricula = funcionariosMarcacoes.Marcacao?.Viatura?.Matricula,
                ViaturaMarca = funcionariosMarcacoes.Marcacao?.Viatura?.Marca,
                ViaturaModelo = funcionariosMarcacoes.Marcacao?.Viatura?.Modelo,
                // Dados do cliente
                ClienteNome = funcionariosMarcacoes.Marcacao?.Viatura?.Cliente?.Nome,
                ClienteTelefone = funcionariosMarcacoes.Marcacao?.Viatura?.Cliente?.Telefone
            };
        }
    }
}
