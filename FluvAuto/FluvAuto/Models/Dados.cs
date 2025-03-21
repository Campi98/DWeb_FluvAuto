using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluvAuto.Models
{
    public class Dados
    {
        public int DadosId { get; set; }

        // FK para referenciar a marcação do serviço
        [ForeignKey(nameof(Marcacao))]
        public int MarcacaoFK { get; set; }
        public Marcacao Marcacao { get; set; }

        // FK para referenciar o funcionário que realizou o serviço
        [ForeignKey(nameof(Funcionario))]
        public int FuncionarioFK { get; set; }
        public Funcionario Funcionario { get; set; }

        [Required]
        public decimal HorasGastas { get; set; }

        [StringLength(500)]
        public string Comentarios { get; set; }
    }
}
