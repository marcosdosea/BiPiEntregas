namespace BipiEntregasWeb.Models
{
    public class CadastrosPendentesViewModel
    {
        public uint Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Cpf { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Veiculo { get; set; }

        public string? Placa { get; set; }

        public string? Cnh { get; set; }

        public bool PossuiDocumentoVeiculo { get; set; }
    }
}
