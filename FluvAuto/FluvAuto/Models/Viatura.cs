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
        // TODO: (ErrorMessage = "A {0} é de preenchimento obrigatório") e [Display(Name = "Marca")] e fazer para todos
        [Required]
        [Display(Name = "Marca")]
        [StringLength(50)]
        public string Marca { get; set; }

        /// <summary>
        /// Modelo da viatura
        /// </summary>
        [Required]
        [Display(Name = "Modelo")]
        [StringLength(50)]
        public string Modelo { get; set; }

        /// <summary>
        /// Matrícula da viatura
        /// </summary>
        [Required]
        [Display(Name = "Matrícula")]
        [StringLength(20)]
        public string Matricula { get; set; }

        /// <summary>
        /// Ano de fabrico da viatura
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [YearRange(1885, ErrorMessage = "O {0} deve estar entre {1} e o ano atual.")]
        [Display(Name = "Ano")]
        public int Ano { get; set; }

        /// <summary>
        /// Lista de marcações associadas à viatura
        /// </summary>
        public ICollection<Marcacao> Marcacoes { get; set; }
        /// <summary>
        /// Cor da viatura
        /// </summary>
        [Required]
        [Display(Name = "Cor")]
        [StringLength(30)]
        public string Cor { get; set; }

        /// <summary>
        /// Tipo de combustível utilizado pela viatura
        /// </summary>
        [Required]
        [Display(Name = "Combustível")]
        [StringLength(20)]
        public string Combustivel { get; set; }

        /// <summary>
        /// Motorização da viatura
        /// </summary>
        [Required]
        [Display(Name = "Motorização")]
        [StringLength(50)]
        public string Motorizacao { get; set; }

        /// <summary>
        /// FK para referenciar o cliente proprietário da viatura
        /// </summary>
        [ForeignKey(nameof(Cliente))]
        [Display(Name = "Cliente")]
        public int ClienteFK { get; set; }
        public Cliente Cliente { get; set; }
    }

    /// <summary>
    /// Atributo personalizado para validar um ano entre um mínimo e o ano atual
    /// </summary>
    public class YearRangeAttribute : RangeAttribute
    {
        public YearRangeAttribute(int minimum)
            : base(minimum, DateTime.Now.Year)
        {
        }
    }

}
