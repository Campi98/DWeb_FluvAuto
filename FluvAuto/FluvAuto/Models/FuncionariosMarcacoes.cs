using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FluvAuto.Models
{
    /// <summary>
    /// Classe que representa os dados de um serviço realizado
    /// </summary>
    [PrimaryKey(nameof(MarcacaoFK),nameof(FuncionarioFK))]
    public class FuncionariosMarcacoes
    {
        /// <summary>
        /// Identificador único dos dados do serviço
        /// </summary>
       // public int DadosId { get; set; }            //TODO: ver como tratar as [Key]s - se será uma composta das duas FKs ou se será só um ID

        /// <summary>
        /// Horas gastas para realizar o serviço
        /// </summary>
        [Required]
        [Display(Name = "Horas Gastas")]
        public decimal HorasGastas { get; set; }

        /// <summary>
        /// Comentários adicionais sobre o serviço / notas do mecânico
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Comentários")]
        public string Comentarios { get; set; }

        /// <summary>
        /// Data e hora do ínicio do serviço
        /// </summary>
        [Required]
        [Display(Name = "Data de Início do Serviço")]
        public DateTime DataInicioServico { get; set; }

        /// <summary>
        /// FK para referenciar a marcação do serviço
        /// </summary>
        [ForeignKey(nameof(Marcacao))]
        [Display(Name = "Marcação")]
        public int MarcacaoFK { get; set; }
        public Marcacao Marcacao { get; set; }

        /// <summary>
        /// FK para referenciar o funcionário que realizou o serviço
        /// </summary>
        [ForeignKey(nameof(Funcionario))]
        [Display(Name = "Funcionário")]
        public int FuncionarioFK { get; set; }
        public Funcionario Funcionario { get; set; }

    }
}
