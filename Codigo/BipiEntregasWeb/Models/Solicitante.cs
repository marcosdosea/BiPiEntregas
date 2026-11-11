using System.ComponentModel.DataAnnotations;

namespace BipiEntregasWeb.Models
{
    public class Solicitante
    {
        [Display(Name = "CEP")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(9, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
        public string Cep { get; set; } = null!;

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
        public string Cidade { get; set; } = null!;

        [Display(Name = "Rua")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
        public string Rua { get; set; } = null!;

        [Display(Name = "Número")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(10, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
        public string Numero { get; set; } = null!;

        [Display(Name = "Complemento")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
        public string? Complemento { get; set; }

        [Display(Name = "Foto da Residência")]
        public string? FotoResidencia { get; set; }
    }
}