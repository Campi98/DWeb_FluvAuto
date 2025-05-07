using System.ComponentModel.DataAnnotations;

namespace FluvAuto.Models
{
    /// <summary>
    /// Classe que representa um cliente
    /// </summary>
    public class Cliente : Utilizador
    {
        /// <summary>
        /// Identificador único do cliente
        /// </summary>
        [Key]
        public int ClienteId { get; set; }

        /// <summary>
        /// Número de Identificação Fiscal (NIF) do cliente
        /// </summary>
        [Display(Name = "NIF")]
        [StringLength(9)]
        [RegularExpression("[1-9][0-9]{8}", ErrorMessage = "Deve escrever apenas 9 dígitos no {0}")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
        public string NIF { get; set; }

        /// <summary>
        /// Lista das viaturas do cliente
        /// </summary>
        public ICollection<Viatura> Viaturas { get; set; }
    }
}
