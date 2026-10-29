using System;
using System.Collections.Generic;

namespace Core;

public partial class Solicitacaoentrega
{
    public uint Id { get; set; }

    public string RuaOrigem { get; set; } = null!;

    public string RuaDestino { get; set; } = null!;

    public string DescricaoItem { get; set; } = null!;

    public double Peso { get; set; }

    public string? Dimensoes { get; set; }

    public decimal ValorFrete { get; set; }

    public string Status { get; set; } = null!;

    public DateTime DataCriacao { get; set; }

    public string NumeroDestino { get; set; } = null!;

    public string NumeroOrigem { get; set; } = null!;

    public string BairroOrigem { get; set; } = null!;

    public string BairroDestino { get; set; } = null!;

    public string CidadeOrigem { get; set; } = null!;

    public string CidadeDestino { get; set; } = null!;

    public string CepOrigem { get; set; } = null!;

    public string CepDestino { get; set; } = null!;

    public int IdSolicitante { get; set; }

    public int? IdEntregador { get; set; }

    public virtual Pessoa? IdEntregadorNavigation { get; set; }

    public virtual Pessoa IdSolicitanteNavigation { get; set; } = null!;

    public virtual ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
}
