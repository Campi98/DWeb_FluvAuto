namespace FluvAuto.Models.ViewModels
{
    public class ViaturasDTO
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
    }
}
