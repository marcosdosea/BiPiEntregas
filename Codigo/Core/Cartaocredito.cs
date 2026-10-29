using System;
using System.Collections.Generic;

namespace Core;

public partial class Cartaocredito
{
    public int Id { get; set; }

    public string NumeroCartao { get; set; } = null!;

    public string NomeCartao { get; set; } = null!;

    public string Validade { get; set; } = null!;

    public string Cvv { get; set; } = null!;

    /// <summary>
    /// CREDITO = C
    /// DEBITO = D
    /// </summary>
    public string TipoCartao { get; set; } = null!;

    public int IdPessoa { get; set; }

    public virtual Pessoa IdPessoaNavigation { get; set; } = null!;
}
