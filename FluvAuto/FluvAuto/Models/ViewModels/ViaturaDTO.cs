namespace FluvAuto.Models.ViewModels
{
    /// <summary>
    /// DTO para transferir dados de viaturas via API, evitando referências circulares
    /// </summary>
    public class ViaturaDTO
    {
        public int ViaturaId { get; set; }
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";
        public string Matricula { get; set; } = "";
        public int Ano { get; set; }
        public string Cor { get; set; } = "";
        public string Combustivel { get; set; } = "";
        public string Motorizacao { get; set; } = "";
        public int ClienteFK { get; set; }

        // Dados do cliente (sem referência circular)
        public string? ClienteNome { get; set; }
        public string? ClienteEmail { get; set; }
        public string? ClienteTelefone { get; set; }

        // Número de marcações (sem incluir os dados completos)
        public int NumeroMarcacoes { get; set; }
    }
}
