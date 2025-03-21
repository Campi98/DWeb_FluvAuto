using System.ComponentModel.DataAnnotations;

namespace FluvAuto.Models
{
    public class Funcionario
    {
        [Key]
        public int FuncionarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(50)]
        public string Funcao { get; set; } // Mecânico, Rececionista, etc.

        [Required]
        [StringLength(100)]
        //[EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        [Phone]
        public string Telefone { get; set; }

        /// <summary>
        /// Lista dos serviços realizados pelo funcionário
        /// </summary>
        public ICollection<Dados> ServicosRealizados { get; set; }
    }
}
