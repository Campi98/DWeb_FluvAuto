using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluvAuto.Models
{
    public class Marcacao
    {
        [Key]
        public int MarcacaoId { get; set; }

        [Required]
        public DateTime DataMarcacao { get; set; }

        [Required]
        [StringLength(50)]
        public string Servico { get; set; }

        [StringLength(500)]
        public string Descricao { get; set; }

        public string Estado { get; set; } // ex: Agendada, Em Progresso, Concluída, Cancelada

        // FK para referenciar a viatura da marcação
        [ForeignKey(nameof(Viatura))]
        public int ViaturaFK { get; set; }
        public Viatura Viatura { get; set; }

        /// <summary>
        /// Lista dos detalhes de serviços associados à marcação
        /// </summary>
        public ICollection<Dados> DadosServicos { get; set; }
    }
}
