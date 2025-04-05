using System.ComponentModel.DataAnnotations;

namespace FluvAuto.Models
{
    /// <summary>
    /// Classe que representa um funcionário da oficina
    /// </summary>
    public class Funcionario
    {
        /// <summary>
        /// Identificador único do funcionário
        /// </summary>
        [Key]
        public int FuncionarioId { get; set; }

        /// <summary>
        /// Nome do funcionário
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        /// <summary>
        /// Função do funcionário na empresa/oficina (Mecânico, Rececionista, etc.)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Funcao { get; set; }

        /// <summary>
        /// Email do funcionário
        /// </summary>
        [Required]
        [StringLength(100)]
        //[EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Número de telefone do funcionário
        /// </summary>
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
