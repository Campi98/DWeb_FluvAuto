namespace FluvAuto.Models.ViewModels
{
    /// <summary>
    /// DTO para transferir dados de marcações via API, evitando problemas de serialização
    /// </summary>
    public class MarcacaoDTO
    {
        public int MarcacaoId { get; set; }
        public DateTime DataMarcacaoFeita { get; set; }
        public DateTime DataPrevistaInicioServico { get; set; }
        public DateTime? DataFimServico { get; set; }
        public string Servico { get; set; } = "";
        public string Observacoes { get; set; } = "";
        public string Estado { get; set; } = "Agendada";
        public int ViaturaFK { get; set; }
        
        // Dados da viatura (sem referência circular)
        public string? ViaturaMatricula { get; set; }
        public string? ViaturaMarca { get; set; }
        public string? ViaturaModelo { get; set; }
        public int? ViaturaAno { get; set; }
        
        // Dados do cliente (sem referência circular)
        public string? ClienteNome { get; set; }
        public string? ClienteEmail { get; set; }
        public string? ClienteTelefone { get; set; }
        
        // Número de serviços associados à marcação
        public int NumeroServicos { get; set; }
    }
}
