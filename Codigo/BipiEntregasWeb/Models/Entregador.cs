namespace BipiEntregasWeb.Models
{
    public class Entregador : Pessoa
    {
        public string Veiculo { get; set; } = "";
        public string Placa { get; set; } = "";
        public string Cnh { get; set; } = "";
        public string DocumentoVeiculo { get; set; } = "";
        public string Status { get; set; } = "";

        public bool Validar()
        {
            return !string.IsNullOrWhiteSpace(Cpf)
                && !string.IsNullOrWhiteSpace(Nome)
                && !string.IsNullOrWhiteSpace(Veiculo)
                && !string.IsNullOrWhiteSpace(Placa)
                && !string.IsNullOrWhiteSpace(Cnh);
        }
    }
}
