using System.ComponentModel.DataAnnotations;

namespace FluvAuto.Models
{
    /// <summary>
    /// Classe que representa um cliente
    /// </summary>
    public class Cliente
    {
        /// <summary>
        /// Identificador único do cliente
        /// </summary>
        [Key]
        public int ClienteId { get; set; }

        /// <summary>
        /// Nome do cliente
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Email do cliente
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [StringLength(100)]
        // [EmailAddress(ErrorMessage = "Introduza um {0} válido.")]                TODO: podemos usar isto?
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Número de telefone do cliente
        /// </summary>
        //[Phone(ErrorMessage = "Introduza um número de {0} válido.")]             TODO: podemos usar isto?
        [Display(Name = "Telemóvel")]
        [StringLength(18)]
        [RegularExpression("(([+]|00)[0-9]{1,5})?[1-9][0-9]{5,10}", ErrorMessage = "Escreva um nº de telefone. Pode adicionar indicativo do país.")]
        public string Telefone { get; set; }
        /*  9[1236][0-9]{7}  --> nºs telemóvel nacional
         *  (([+]|00)[0-9]{1,5})?[1-9][0-9]{5,10}  -->  nºs telefone internacionais
         */

        /// <summary>
        /// Morada do cliente
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Morada")]
        public string Morada { get; set; }

        /// <summary>
        /// Código Postal da  morada do utilizador
        /// </summary>
        [Display(Name = "Código Postal")]
        [StringLength(50)]
        [RegularExpression("[1-9][0-9]{3}-[0-9]{3} [A-Za-z ]+", ErrorMessage = "No {0} só são aceites algarismos e letras inglesas.")]
        public string CodPostal { get; set; }
        /* exemplo de exp. regulares sobre o Código Postal
         * [1-9][0-9]{3}-[0-9]{3} [A-Za-z ]+  --> Portugal
         * [1-9][0-9]{3,4}-[0-9]{3,4}( [A-Za-z ]*)?  --> fora de Portugal
         */

        /// <summary>
        /// Número de Identificação Fiscal (NIF) do cliente
        /// </summary>
        [Display(Name = "NIF")]
        [StringLength(9)]
        [RegularExpression("[1-9][0-9]{8}", ErrorMessage = "Deve escrever apenas 9 digitos no {0}")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
        public string NIF { get; set; }

        /// <summary>
        /// Lista das viaturas do cliente
        /// </summary>
        public ICollection<Viatura> Viaturas { get; set; }
    }
}
