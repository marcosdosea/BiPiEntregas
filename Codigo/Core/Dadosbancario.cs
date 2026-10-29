using System;
using System.Collections.Generic;

namespace Core;

public partial class Dadosbancario
{
    public uint Id { get; set; }

    public string Agencia { get; set; } = null!;

    /// <summary>
    /// CONTA CORRENTE = C
    /// CONTA POPANÇA =  P
    /// </summary>
    public string Conta { get; set; } = null!;

    public string NomeTitular { get; set; } = null!;

    public string CpfTitular { get; set; } = null!;

    public string TipoConta { get; set; } = null!;

    public int IdPessoa { get; set; }

    public int IdBanco { get; set; }

    public virtual Banco IdBancoNavigation { get; set; } = null!;

    public virtual Pessoa IdPessoaNavigation { get; set; } = null!;
}
