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
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
        [Display(Name = "Marca")]
        [StringLength(30)]
        public string Marca { get; set; } = "";

        /// <summary>
        /// Modelo da viatura
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [Display(Name = "Modelo")]
        [StringLength(50)]
        public string Modelo { get; set; } = "";

        /// <summary>
        /// Matrícula da viatura
        /// </summary>
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
        [Display(Name = "Matrícula")]
        [StringLength(20)]
        [RegularExpression(@"^(([A-Za-z]{2}-[A-Za-z]{2}-[0-9]{2})|([0-9]{2}-[0-9]{2}-[A-Za-z]{2})|([A-Za-z]{2}-[0-9]{2}-[A-Za-z]{2})|([0-9]{2}-[A-Za-z]{2}-[0-9]{2})|([A-Za-z]{2}-[0-9]{2}-[0-9]{2})|([0-9]{2}-[A-Za-z]{2}-[A-Za-z]{2}))$",
            ErrorMessage = "A matrícula deve ter o formato XX-YY-ZZ, com máximo de 4 letras e 2 números ou 4 números e 2 letras.")]
        public string Matricula { get; set; } = "";

        /// <summary>
        /// Ano de fabrico da viatura
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [YearRange(1885, ErrorMessage = "O {0} deve estar entre {1} e o ano atual.")]
        [Display(Name = "Ano")]
        public int Ano { get; set; }

        /// <summary>
        /// Cor da viatura
        /// </summary>
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
        [Display(Name = "Cor")]
        [StringLength(30)]
        public string Cor { get; set; } = "";

        /// <summary>
        /// Tipo de combustível da viatura
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [Display(Name = "Combustível")]
        [RegularExpression("^(Gasolina|Diesel|Elétrico|Híbrido|GPL)$",
            ErrorMessage = "O combustível deve ser Gasolina, Diesel, GPL, Elétrico ou Híbrido")]
        public string Combustivel { get; set; } = "";

        /// <summary>
        /// Motorização da viatura
        /// </summary>
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
        [Display(Name = "Motorização")]
        [StringLength(50)]
        public string Motorizacao { get; set; } = "";

        /// <summary>
        /// FK para referenciar o cliente proprietário da viatura
        /// </summary>
        [ForeignKey(nameof(Cliente))]
        [Display(Name = "Cliente")]
        public int ClienteFK { get; set; }
        public Cliente Cliente { get; set; }

        /// <summary>
        /// Lista de marcações associadas à viatura
        /// </summary>
        public ICollection<Marcacao> Marcacoes { get; set; } = new List<Marcacao>();
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
