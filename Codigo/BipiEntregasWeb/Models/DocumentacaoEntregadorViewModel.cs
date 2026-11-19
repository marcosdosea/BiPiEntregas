
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BipiEntregasWeb.Models
{
    public class DocumentacaoEntregadorViewModel
    {
        [Required(ErrorMessage = "Informe a CNH.")]
        [StringLength(15)]
        public string Cnh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o veículo.")]
        [StringLength(20)]
        public string Veiculo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a placa.")]
        [StringLength(7, MinimumLength = 7)]
        public string Placa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Envie o documento do veículo.")]
        [DataType(DataType.Upload)]
        public IFormFile? DocumentoVeiculo { get; set; }
    }
}
