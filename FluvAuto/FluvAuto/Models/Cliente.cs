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
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        /// <summary>
        /// Email do cliente
        /// </summary>
        [Required]
        [StringLength(100)]
        //[EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Número de telefone do cliente
        /// </summary>
        [Required]
        [StringLength(20)]
        //[Phone]
        public string Telefone { get; set; }

        /// <summary>
        /// Morada do cliente
        /// </summary>
        [StringLength(200)]
        public string Morada { get; set; }

        /// <summary>
        /// Número de Identificação Fiscal (NIF) do cliente
        /// </summary>
        [StringLength(20)]
        public string NIF { get; set; }

        /// <summary>
        /// Lista das viaturas do cliente
        /// </summary>
        public ICollection<Viatura> Viaturas { get; set; }
    }
}
