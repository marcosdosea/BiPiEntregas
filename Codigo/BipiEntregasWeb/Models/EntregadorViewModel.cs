
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BipiEntregasWeb.Models
{
    public class EntregadorViewModel
    {
        [Required(ErrorMessage = "Informe o veículo.")]
        [Display(Name = "Veículo")]
        public string? Veiculo { get; set; }

        [Required(ErrorMessage = "Informe a placa do veículo.")]
        [Display(Name = "Placa do veículo")]
        public string? Placa { get; set; }

        [Required(ErrorMessage = "Informe a CNH.")]
        [Display(Name = "CNH")]
        public string? Cnh { get; set; }

        [Required(ErrorMessage = "Envie o documento do veículo.")]
        [Display(Name = "Documento do veículo")]
        public IFormFile? DocumentoVeiculo { get; set; }
    }
}
