using System;
using System.Collections.Generic;

namespace Core;

public partial class Pagamento
{
    public uint Id { get; set; }

    public decimal Valor { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? DataPagamento { get; set; }

    public DateTime? DataEstorno { get; set; }

    public string Tipo { get; set; } = null!;

    public uint IdSolicitacaoEntrega { get; set; }

    public virtual Solicitacaoentrega IdSolicitacaoEntregaNavigation { get; set; } = null!;
}
