using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluvAuto.Models
{
    /// <summary>
    /// Classe que representa uma marcação de serviço
    /// </summary>
    public class Marcacao
    {
        /// <summary>
        /// Identificador único da marcação
        /// </summary>
        [Key]
        public int MarcacaoId { get; set; }

        /// <summary>
        /// Data e hora da marcação
        /// </summary>
        [Required]
        [Display(Name = "Data de Marcação")]
        public DateTime DataMarcacaoFeita { get; set; }


        /// <summary>
        /// Data e hora prevista para o início do serviço
        /// </summary>
        [Display(Name = "Data Prevista para Início")]
        public DateTime DataPrevistaInicioServico { get; set; }

        /// <summary>
        /// Data e hora do fim do serviço
        /// </summary>
        [Display(Name = "Data de Conclusão")]
        public DateTime? DataFimServico { get; set; }

        /// <summary>
        /// Serviço a ser realizado (ex: troca de óleo, revisão geral, alinhar direção etc.)
        /// </summary>
        [Required]
        [StringLength(200)]
        [Display(Name = "Serviço(s)")]
        public string Servico { get; set; }         //TODO: isto seria só uma textarea? ou uma lista de serviços?

        /// <summary>
        /// Descrição adicional sobre a marcação, por parte do cliente
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Observações")]
        public string Observacoes { get; set; }

        /// <summary>
        /// Estado da marcação (ex: Agendada, Em Progresso, Concluída, Cancelada)
        /// </summary>
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        /// <summary>
        /// FK para referenciar a viatura da marcação
        /// </summary>
        [ForeignKey(nameof(Viatura))]
        [Display(Name = "Viatura")]
        public int ViaturaFK { get; set; }
        public Viatura Viatura { get; set; }

        /// <summary>
        /// Lista dos detalhes de serviços associados à marcação
        /// </summary>
        public ICollection<FuncionariosMarcacoes> DadosServicos { get; set; }       //TODO: isto existe por causa do DERE que tínhamos anteriormente? VER se é para remover
    }
}
