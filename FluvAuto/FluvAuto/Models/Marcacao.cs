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
        public DateTime DataMarcacao { get; set; }

        /// <summary>
        /// Serviço a ser realizado (ex: troca de óleo, revisão geral, alinhar direção etc.)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Servico { get; set; }         //TODO: isto seria um enum? temos de ter uma tabela intermediária para os serviços?

        /// <summary>
        /// Descrição adicional sobre a marcação
        /// </summary>
        [StringLength(500)]
        public string Descricao { get; set; }       //TODO: isto é quê? melhorar summary

        /// <summary>
        /// Estado da marcação (ex: Agendada, Em Progresso, Concluída, Cancelada)
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// FK para referenciar a viatura da marcação
        /// </summary>
        [ForeignKey(nameof(Viatura))]
        public int ViaturaFK { get; set; }
        public Viatura Viatura { get; set; }

        /// <summary>
        /// Lista dos detalhes de serviços associados à marcação
        /// </summary>
        public ICollection<Dados> DadosServicos { get; set; }
    }
}
