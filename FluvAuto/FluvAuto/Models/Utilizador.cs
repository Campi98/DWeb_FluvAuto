using System.ComponentModel.DataAnnotations;

namespace FluvAuto.Models
{
    /// <summary>
    /// Classe base que representa um utilizador do sistema
    /// </summary>
    public class Utilizador
    {
        /// <summary>
        /// Identificador único do Utilizador
        /// </summary>
        [Key]
        public int Id { get; set; }


        public string UserName { get; set; }      // ligação identity - local

        /// <summary>
        /// Nome do utilizador
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Email do utilizador
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [StringLength(100)]
        // [EmailAddress(ErrorMessage = "Introduza um {0} válido.")]                TODO: podemos usar isto?
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Número de telefone do utilizador
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
        /// Morada do utilizador
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Morada")]
        public string Morada { get; set; }

        /// <summary>
        /// Código Postal da morada do utilizador
        /// </summary>
        [Display(Name = "Código Postal")]
        [StringLength(50)]
        [RegularExpression("[1-9][0-9]{3}-[0-9]{3} [A-Za-z ]+", ErrorMessage = "No {0} só são aceites algarismos e letras inglesas.")]
        public string CodPostal { get; set; }
        /* exemplo de exp. regulares sobre o Código Postal
         * [1-9][0-9]{3}-[0-9]{3} [A-Za-z ]+  --> Portugal
         * [1-9][0-9]{3,4}-[0-9]{3,4}( [A-Za-z ]*)?  --> fora de Portugal
         */
    }
}
