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
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
        [Display(Name = "Data de Marcação")]
        public DateTime DataMarcacaoFeita { get; set; }

        /// <summary>
        /// Data e hora prevista para o início do serviço
        /// </summary>
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
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
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [StringLength(200)]
        [Display(Name = "Serviço(s)")]
        public string Servico { get; set; } = "";

        /// <summary>
        /// Descrição adicional sobre a marcação, por parte do cliente
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Observações (opcional)")]
        public string Observacoes { get; set; } = "";

        /// <summary>
        /// Estado da marcação (ex: Agendada, Em Progresso, Concluída, Cancelada)
        /// </summary>
        [Display(Name = "Estado")]
        [RegularExpression("^(Agendada|Em Progresso|Concluída|Cancelada)$",
            ErrorMessage = "O estado deve ser: Agendada, Em Progresso, Concluída ou Cancelada")]
        public string Estado { get; set; } = "Agendada";

        /// <summary>
        /// FK para referenciar a viatura da marcação
        /// </summary>
        [ForeignKey(nameof(Viatura))]
        [Display(Name = "Viatura")]
        public int ViaturaFK { get; set; }
        public Viatura? Viatura { get; set; }

        /// <summary>
        /// Lista dos detalhes de serviços associados à marcação
        /// </summary>
        public ICollection<FuncionariosMarcacoes> DadosServicos { get; set; } = new List<FuncionariosMarcacoes>();
    }
}