using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluvAuto.Models
{
    /// <summary>
    /// Classe que representa os dados de um serviço realizado
    /// </summary>
    public class Dados
    {
        /// <summary>
        /// Identificador único dos dados do serviço
        /// </summary>
        public int DadosId { get; set; }

        /// <summary>
        /// Horas gastas para realizar o serviço
        /// </summary>
        [Required]
        public decimal HorasGastas { get; set; }

        /// <summary>
        /// Comentários adicionais sobre o serviço / notas do mecânico
        /// </summary>
        [StringLength(500)]
        public string Comentarios { get; set; }

        /// <summary>
        /// FK para referenciar a marcação do serviço
        /// </summary>
        [ForeignKey(nameof(Marcacao))]
        public int MarcacaoFK { get; set; }
        public Marcacao Marcacao { get; set; }

        /// <summary>
        /// FK para referenciar o funcionário que realizou o serviço
        /// </summary>
        [ForeignKey(nameof(Funcionario))]
        public int FuncionarioFK { get; set; }
        public Funcionario Funcionario { get; set; }

    }
}
