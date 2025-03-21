using System.ComponentModel.DataAnnotations;

namespace FluvAuto.Models
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(100)]
        //[EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        //[Phone]
        public string Telefone { get; set; }

        [StringLength(200)]
        public string Morada { get; set; }

        [StringLength(20)]
        public string NIF { get; set; }

        /// <summary>
        /// Lista das viaturas do cliente
        /// </summary>
        public ICollection<Viatura> Viaturas { get; set; }
    }
}
