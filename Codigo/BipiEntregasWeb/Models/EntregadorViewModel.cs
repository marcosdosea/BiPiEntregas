using System.ComponentModel.DataAnnotations;


namespace BipiEntregasWeb.Models
{
    public class EntregadorViewModel
    {
        public uint Id { get; set; }

        [Required(ErrorMessage = "Informe o nome.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o CPF.")]
        [StringLength(11, MinimumLength = 11)]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress]
        [StringLength(75)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o telefone.")]
        [StringLength(11)]
        public string Telefone { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "Informe o veículo.")]
        [StringLength(20)]
        public string? Veiculo { get; set; }

        [Required(ErrorMessage = "Informe a placa do veículo.")]
        [StringLength(7)]
        public string? Placa { get; set; }

        [Required(ErrorMessage = "Informe a Cnh.")]
        [StringLength(11)] 
        public string? Cnh { get; set; }

        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Informe o Documento do Veículo.")]
        public IFormFile? DocumentoVeiculo{ get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
