using System.ComponentModel.DataAnnotations;

namespace FluvAuto.Models
{
    /// <summary>
    /// Classe que representa um funcionário da oficina
    /// </summary>
    public class Funcionario : Utilizador
    {
        /// <summary>
        /// Identificador único do funcionário
        /// </summary>
        [Key]
        public int FuncionarioId { get; set; }

        /// <summary>
        /// Função do funcionário na empresa/oficina (Mecânico, Rececionista, etc.)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Funcao { get; set; }

        /// <summary>
        /// Fotografia do funcionário
        /// </summary>
        public string Fotografia { get; set; }

        /// <summary>
        /// Lista dos serviços realizados pelo funcionário
        /// </summary>
        public ICollection<FuncionariosMarcacoes> ServicosRealizados { get; set; }
    }
}
