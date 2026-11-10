using System;
using System.ComponentModel.DataAnnotations;

namespace BipiEntregasWeb.Models
{
    public class Pessoa
    {
        [Key]
        [Required(ErrorMessage = "O campo CPF é obrigatório.")]
        [RegularExpression(@"^\d{11}$",
            ErrorMessage = "O CPF deve conter exatamente 11 números, sem pontuação.")]
        public string Cpf { get; set; } = null!;

        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um email válido.")]
        [StringLength(254,
            ErrorMessage = "O campo Email deve ter no máximo 254 caracteres.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = null!;

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [StringLength(100,
            ErrorMessage = "O campo Nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O campo Telefone é obrigatório.")]
        [RegularExpression(@"^\d{10,11}$",
            ErrorMessage = "O telefone deve conter DDD e número, com 10 ou 11 dígitos.")]
        public string Telefone { get; set; } = null!;

        [Required(ErrorMessage = "O campo Data de Nascimento é obrigatório.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime? DataNascimento { get; set; }
    }
}