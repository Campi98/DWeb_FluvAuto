using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluvAuto.Models
{
    public class Viatura
    {
        [Key]
        public int ViaturaId { get; set; }

        [Required]
        [StringLength(50)]
        public string Marca { get; set; }

        [Required]
        [StringLength(50)]
        public string Modelo { get; set; }

        [Required]
        [StringLength(20)]
        public string Matricula { get; set; }

        public int Ano { get; set; }

        // FK para referenciar o cliente proprietário
        [ForeignKey(nameof(Cliente))]
        public int ClienteFK { get; set; }
        public Cliente Cliente { get; set; }

        /// <summary>
        /// Lista de marcações associadas à viatura
        /// </summary>
        public ICollection<Marcacao> Marcacoes { get; set; }
    }
}
