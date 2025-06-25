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
    public class MarcacoesController : ControllerBase
    {
        private readonly ApplicationDbContext _bd;

        public MarcacoesController(ApplicationDbContext context)
        {
            _bd = context;
        }

        // GET: api/Marcacoes
        /// <summary>
        /// Obtém a lista de marcações. Admin/funcionários veem todas, clientes só as suas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarcacaoDTO>>> GetMarcacoes()
        {
            var query = _bd.Marcacoes
                .Include(m => m.Viatura)
                    .ThenInclude(v => v!.Cliente);

            if (User.IsInRole("admin") || User.IsInRole("funcionario"))
            {
                // Admin e funcionários veem todas as marcações
                var marcacoes = await query.ToListAsync();
                return Ok(marcacoes.Select(MarcacaoToDTO));
            }
            else
            {
                // Cliente: só vê as suas próprias marcações
                var username = User.Identity?.Name;
                var clienteId = _bd.Clientes.Where(c => c.UserName == username).Select(c => c.UtilizadorId).FirstOrDefault();
                
                var marcacoesCliente = await query
                    .Where(m => m.Viatura != null && m.Viatura.ClienteFK == clienteId)
                    .ToListAsync();
                
                return Ok(marcacoesCliente.Select(MarcacaoToDTO));
            }
        }

        // GET: api/Marcacoes/5
        /// <summary>
        /// Obtém uma marcação específica com base no seu ID
        /// </summary>
        /// <param name="id">Identificador da marcação</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MarcacaoDTO>> GetMarcacao(int id)
        {
            var marcacao = await _bd.Marcacoes
                .Include(m => m.Viatura)
                    .ThenInclude(v => v!.Cliente)
                .FirstOrDefaultAsync(m => m.MarcacaoId == id);

            if (marcacao == null)
            {
                return NotFound();
            }

            // Cliente: só pode ver marcações das suas viaturas
            if (!(User.IsInRole("admin") || User.IsInRole("funcionario")))
            {
                var username = User.Identity?.Name;
                var clienteId = _bd.Clientes.Where(c => c.UserName == username).Select(c => c.UtilizadorId).FirstOrDefault();
                if (marcacao.Viatura?.ClienteFK != clienteId)
                {
                    return Forbid();
                }
            }

            return Ok(MarcacaoToDTO(marcacao));
        }

        // PUT: api/Marcacoes/5
        /// <summary>
        /// Atualiza uma marcação existente na base de dados com base no seu ID
        /// </summary>
        /// <param name="id">Identificação da marcação a editar</param>
        /// <param name="marcacaoDTO">Novos dados da marcação</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,funcionario")]
        public async Task<IActionResult> PutMarcacao(int id, MarcacaoDTO marcacaoDTO)
        {
            if (id != marcacaoDTO.MarcacaoId)
            {
                return BadRequest();
            }

            var marcacao = await _bd.Marcacoes.FindAsync(id);
            if (marcacao == null)
            {
                return NotFound();
            }

            marcacao.DataMarcacaoFeita = marcacaoDTO.DataMarcacaoFeita;
            marcacao.DataPrevistaInicioServico = marcacaoDTO.DataPrevistaInicioServico;
            marcacao.DataFimServico = marcacaoDTO.DataFimServico;
            marcacao.Servico = marcacaoDTO.Servico;
            marcacao.Observacoes = marcacaoDTO.Observacoes;
            marcacao.Estado = marcacaoDTO.Estado;
            marcacao.ViaturaFK = marcacaoDTO.ViaturaFK;

            _bd.Entry(marcacao).State = EntityState.Modified;

            try
            {
                await _bd.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarcacaoExists(id))
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

        // POST: api/Marcacoes
        /// <summary>
        /// Cria uma nova marcação na base de dados
        /// </summary>
        /// <param name="marcacaoDTO">Dados da marcação a ser criada</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<MarcacaoDTO>> PostMarcacao(MarcacaoDTO marcacaoDTO)
        {
            // Verificar se a viatura existe e se o cliente tem acesso a ela
            var viatura = await _bd.Viaturas.Include(v => v.Cliente).FirstOrDefaultAsync(v => v.ViaturaId == marcacaoDTO.ViaturaFK);
            if (viatura == null)
            {
                return BadRequest("Viatura não encontrada");
            }

            // Cliente: só pode criar marcações para as suas viaturas
            if (!(User.IsInRole("admin") || User.IsInRole("funcionario")))
            {
                var username = User.Identity?.Name;
                var clienteId = _bd.Clientes.Where(c => c.UserName == username).Select(c => c.UtilizadorId).FirstOrDefault();
                if (viatura.ClienteFK != clienteId)
                {
                    return Forbid();
                }
            }

            var marcacao = new Marcacao
            {
                DataMarcacaoFeita = marcacaoDTO.DataMarcacaoFeita,
                DataPrevistaInicioServico = marcacaoDTO.DataPrevistaInicioServico,
                DataFimServico = marcacaoDTO.DataFimServico,
                Servico = marcacaoDTO.Servico,
                Observacoes = marcacaoDTO.Observacoes,
                Estado = marcacaoDTO.Estado,
                ViaturaFK = marcacaoDTO.ViaturaFK
            };

            _bd.Marcacoes.Add(marcacao);
            await _bd.SaveChangesAsync();

            // Traz a marcação criada com as informações completas
            var marcacaoCriada = await _bd.Marcacoes
                .Include(m => m.Viatura)
                    .ThenInclude(v => v!.Cliente)
                .FirstOrDefaultAsync(m => m.MarcacaoId == marcacao.MarcacaoId);

            return CreatedAtAction("GetMarcacao", new { id = marcacao.MarcacaoId }, MarcacaoToDTO(marcacaoCriada!));
        }

        // DELETE: api/Marcacoes/5
        /// <summary>
        /// Remove uma marcação da base de dados com base no seu ID
        /// </summary>
        /// <param name="id">Identificador da marcação a apagar</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,funcionario")]
        public async Task<IActionResult> DeleteMarcacao(int id)
        {
            var marcacao = await _bd.Marcacoes.FindAsync(id);
            if (marcacao == null)
            {
                return NotFound();
            }

            _bd.Marcacoes.Remove(marcacao);
            await _bd.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica se uma marcação com o ID especificado já existe na base de dados
        /// </summary>
        /// <param name="id">Identificador da marcação a procurar</param>
        /// <returns>'true', se a marcação existe, senão 'false'</returns>
        private bool MarcacaoExists(int id)
        {
            return _bd.Marcacoes.Any(e => e.MarcacaoId == id);
        }

        /// <summary>
        /// Converte uma entidade Marcacao para MarcacaoDTO
        /// </summary>
        /// <param name="marcacao">Entidade Marcacao</param>
        /// <returns>MarcacaoDTO</returns>
        private MarcacaoDTO MarcacaoToDTO(Marcacao marcacao)
        {
            return new MarcacaoDTO
            {
                MarcacaoId = marcacao.MarcacaoId,
                DataMarcacaoFeita = marcacao.DataMarcacaoFeita,
                DataPrevistaInicioServico = marcacao.DataPrevistaInicioServico,
                DataFimServico = marcacao.DataFimServico,
                Servico = marcacao.Servico,
                Observacoes = marcacao.Observacoes,
                Estado = marcacao.Estado,
                ViaturaFK = marcacao.ViaturaFK,
                // Dados da viatura
                ViaturaMatricula = marcacao.Viatura?.Matricula,
                ViaturaMarca = marcacao.Viatura?.Marca,
                ViaturaModelo = marcacao.Viatura?.Modelo,
                ViaturaAno = marcacao.Viatura?.Ano,
                // Dados do cliente
                ClienteNome = marcacao.Viatura?.Cliente?.Nome,
                ClienteEmail = marcacao.Viatura?.Cliente?.Email,
                ClienteTelefone = marcacao.Viatura?.Cliente?.Telefone,
                // Número de serviços associados
                NumeroServicos = _bd.FuncionariosMarcacoes.Count(fm => fm.MarcacaoFK == marcacao.MarcacaoId)
            };
        }
    }
}
