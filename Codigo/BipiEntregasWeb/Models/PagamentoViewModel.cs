using System.ComponentModel.DataAnnotations;

namespace BipiEntregasWeb.Models
{
    public class PagamentoViewModel
    {
        public uint Id { get; set; }

        [Required(ErrorMessage = "Informe o código da solicitação de entrega.")]
        [Range(1, int.MaxValue, ErrorMessage = "Informe o código da solicitação de entrega.")]
        [Display(Name = "Código da Solicitação")]
        public uint IdSolicitacaoEntrega { get; set; }

        [Range(0.01, 99999999, ErrorMessage = "Informe um valor maior que zero.")]
        [Display(Name = "Valor")]
        public decimal Valor { get; set; }

        // P = Pix, C = Cartão
        [Required(ErrorMessage = "Selecione a forma de pagamento.")]
        [RegularExpression("^(P|C)$", ErrorMessage = "Selecione Pix ou Cartão.")]
        [Display(Name = "Forma de Pagamento")]
        public string Tipo { get; set; } = string.Empty;

        // Definidos pelo sistema (Service). Não são recebidos do formulário.
        // P = Pendente, A = Aprovado, R = Recusado, E = Estornado
        [Display(Name = "Situação")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Data do Pagamento")]
        public DateTime? DataPagamento { get; set; }

        [Display(Name = "Data do Estorno")]
        public DateTime? DataEstorno { get; set; }
    }
}