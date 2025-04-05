using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluvAuto.Models
{
    /// <summary>
    /// Classe que representa uma viatura
    /// </summary>
    public class Viatura
    {
        /// <summary>
        /// Identificador único da viatura
        /// </summary>
        [Key]
        public int ViaturaId { get; set; }

        /// <summary>
        /// Marca da viatura
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Marca { get; set; }

        /// <summary>
        /// Modelo da viatura
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Modelo { get; set; }

        /// <summary>
        /// Matrícula da viatura
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Matricula { get; set; }

        /// <summary>
        /// Ano de fabrico da viatura
        /// </summary>
        public int Ano { get; set; }

        /// <summary>
        /// FK para referenciar o cliente proprietário da viatura
        /// </summary>
        [ForeignKey(nameof(Cliente))]
        public int ClienteFK { get; set; }
        public Cliente Cliente { get; set; }

        /// <summary>
        /// Lista de marcações associadas à viatura
        /// </summary>
        public ICollection<Marcacao> Marcacoes { get; set; }
    }
}
