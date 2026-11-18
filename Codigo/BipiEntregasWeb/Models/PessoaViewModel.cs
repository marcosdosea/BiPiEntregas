using System.ComponentModel.DataAnnotations;

namespace BipiEntregasWeb.Models
{
    public class PessoaViewModel
    {
        public uint Id { get; set; }

        [Required(ErrorMessage = "Informe o nome.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o CPF.")]
        [StringLength(11, MinimumLength = 11)]
        [Display(Name = "CPF")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress]
        [StringLength(75)]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o telefone.")]
        [StringLength(11)]
        public string Telefone { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        // S = Solicitante, E = Entregador, A = Administrador
        [Required(ErrorMessage = "Selecione o perfil.")]
        [RegularExpression("^(S|E|A)$", ErrorMessage = "Selecione um perfil válido.")]
        [Display(Name = "Perfil")]
        public string Papel { get; set; } = string.Empty;

        // A = Ativo, I = Inativo, P = Pendente
        [Display(Name = "Situação")]
        public string Status { get; set; } = string.Empty;
    }
}