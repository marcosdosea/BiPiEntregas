using System.ComponentModel.DataAnnotations;

namespace BipiEntregasWeb.Models
{
    public class RejeitarCadastroViewModel
    {
        [Required(ErrorMessage = "Informe o motivo da rejeição.")]
        [StringLength(
            500,
            ErrorMessage = "O motivo deve ter no máximo 500 caracteres.")]
        public string Motivo { get; set; } = string.Empty;
    }
}
