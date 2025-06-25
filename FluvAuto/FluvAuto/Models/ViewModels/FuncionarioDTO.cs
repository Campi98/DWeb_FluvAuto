namespace FluvAuto.Models.ViewModels
{
    /// <summary>
    /// DTO para transferir dados de funcionários via API, evitando problemas de serialização
    /// </summary>
    public class FuncionarioDTO
    {
        public int UtilizadorId { get; set; }
        public string UserName { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public string Telefone { get; set; } = "";
        public string? Morada { get; set; }
        public string? CodPostal { get; set; }
        public string Funcao { get; set; } = "";
        public string Fotografia { get; set; } = "";
        
        // Número de serviços realizados (sem incluir os dados completos)
        public int NumeroServicosRealizados { get; set; }
    }
}
