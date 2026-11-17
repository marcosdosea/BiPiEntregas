
using System.ComponentModel.DataAnnotations;

namespace BipiEntregasWeb.Models
{
    public class SolicitanteViewModel
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

        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string Cep { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Cidade { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Rua { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Numero { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Complemento { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
