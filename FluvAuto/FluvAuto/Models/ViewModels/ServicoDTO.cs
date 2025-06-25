namespace FluvAuto.Models.ViewModels
{
    /// <summary>
    /// DTO para transferir dados de serviços (FuncionariosMarcacoes) via API, evitando problemas de serialização
    /// </summary>
    public class ServicoDTO
    {
        public int MarcacaoFK { get; set; }
        public int FuncionarioFK { get; set; }
        public decimal HorasGastas { get; set; }
        public string Comentarios { get; set; } = "";
        public DateTime DataInicioServico { get; set; }
        
        // Dados da marcação (sem referência circular)
        public string? MarcacaoServico { get; set; }
        public DateTime? MarcacaoDataPrevista { get; set; }
        public DateTime? MarcacaoDataFim { get; set; }
        public string? MarcacaoEstado { get; set; }
        
        // Dados do funcionário (sem referência circular)
        public string? FuncionarioNome { get; set; }
        public string? FuncionarioFuncao { get; set; }
        
        // Dados da viatura (sem referência circular)
        public string? ViaturaMatricula { get; set; }
        public string? ViaturaMarca { get; set; }
        public string? ViaturaModelo { get; set; }
        
        // Dados do cliente (sem referência circular)
        public string? ClienteNome { get; set; }
        public string? ClienteTelefone { get; set; }
    }
}
