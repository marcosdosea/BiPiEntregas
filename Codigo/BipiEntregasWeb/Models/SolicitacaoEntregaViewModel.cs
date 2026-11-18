using System.ComponentModel.DataAnnotations;

namespace BipiEntregasWeb.Models
{
    public class SolicitacaoEntregaViewModel
    {
        public uint Id { get; set; }

        [Required(ErrorMessage = "Informe o código do solicitante.")]
        [Range(1, int.MaxValue, ErrorMessage = "Informe o código do solicitante.")]
        [Display(Name = "Código do Solicitante")]
        public uint IdSolicitante { get; set; }

        [Required(ErrorMessage = "Informe a rua de origem.")]
        [StringLength(100)]
        [Display(Name = "Rua de Origem")]
        public string RuaOrigem { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o número de origem.")]
        [StringLength(100)]
        [Display(Name = "Número de Origem")]
        public string NumeroOrigem { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o bairro de origem.")]
        [StringLength(100)]
        [Display(Name = "Bairro de Origem")]
        public string BairroOrigem { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a cidade de origem.")]
        [StringLength(100)]
        [Display(Name = "Cidade de Origem")]
        public string CidadeOrigem { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o CEP de origem.")]
        [StringLength(8, MinimumLength = 8)]
        [Display(Name = "CEP de Origem")]
        public string CepOrigem { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a rua de destino.")]
        [StringLength(100)]
        [Display(Name = "Rua de Destino")]
        public string RuaDestino { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o número de destino.")]
        [StringLength(100)]
        [Display(Name = "Número de Destino")]
        public string NumeroDestino { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o bairro de destino.")]
        [StringLength(100)]
        [Display(Name = "Bairro de Destino")]
        public string BairroDestino { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a cidade de destino.")]
        [StringLength(100)]
        [Display(Name = "Cidade de Destino")]
        public string CidadeDestino { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o CEP de destino.")]
        [StringLength(8, MinimumLength = 8)]
        [Display(Name = "CEP de Destino")]
        public string CepDestino { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a descrição do item.")]
        [StringLength(255)]
        [Display(Name = "Descrição do Item")]
        public string DescricaoItem { get; set; } = string.Empty;

        [Range(0.01, 999999, ErrorMessage = "Informe um peso válido.")]
        [Display(Name = "Peso (kg)")]
        public decimal Peso { get; set; }

        [Required(ErrorMessage = "Informe as dimensões.")]
        [StringLength(30)]
        [Display(Name = "Dimensões")]
        public string Dimensoes { get; set; } = string.Empty;

        // Definidos pelo sistema (Service). Não são recebidos do formulário.
        [Display(Name = "Valor do Frete")]
        public decimal ValorFrete { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime DataCriacao { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
