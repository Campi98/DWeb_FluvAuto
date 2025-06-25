namespace FluvAuto.Models.ViewModels
{
    /// <summary>
    /// DTO para transferir dados de clientes via API, evitando problemas de serialização
    /// </summary>
    public class ClienteDTO
    {
        public int UtilizadorId { get; set; }
        public string UserName { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public string Telefone { get; set; } = "";
        public string? Morada { get; set; }
        public string? CodPostal { get; set; }
        public string NIF { get; set; } = "";

        // Número de viaturas (sem incluir os dados completos)
        public int NumeroViaturas { get; set; }
    }
}
