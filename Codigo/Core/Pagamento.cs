using System;
using System.Collections.Generic;

namespace Core;

public partial class Pagamento
{
    public int Id { get; set; }

    public decimal Valor { get; set; }

    /// <summary>
    /// PENDENTE = P
    /// APROVADO = A
    /// RECUSADO = R
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime DataPagamento { get; set; }

    /// <summary>
    /// PIX = P
    /// CARTÃO DE CREDITO = A
    /// </summary>
    public string Tipo { get; set; } = null!;

    public uint IdSolicitacaoEntrega { get; set; }

    public virtual Solicitacaoentrega IdSolicitacaoEntregaNavigation { get; set; } = null!;
}
