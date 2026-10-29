using System;
using System.Collections.Generic;

namespace Core;

public partial class Pessoa
{
    public int Id { get; set; }

    public string Cpf { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public DateTime DataNascimento { get; set; }

    public string Cep { get; set; } = null!;

    public string Cidade { get; set; } = null!;

    public string Rua { get; set; } = null!;

    public string Numero { get; set; } = null!;

    public string? Complemento { get; set; }

    public byte[]? FotoResidencia { get; set; }

    public string? Veiculo { get; set; }

    public string? Placa { get; set; }

    public string? Cnh { get; set; }

    public byte[]? DocumentoVeiculo { get; set; }

    /// <summary>
    /// A - ATIVO
    /// I - INATIVO
    /// P - PENDENTE
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// SOLICITANTE = S
    /// ENTREGADOR = E
    /// ADMINISTRADOR = A
    /// </summary>
    public string Papel { get; set; } = null!;

    public virtual ICollection<Cartaocredito> Cartaocreditos { get; set; } = new List<Cartaocredito>();

    public virtual ICollection<Dadosbancario> Dadosbancarios { get; set; } = new List<Dadosbancario>();

    public virtual ICollection<Solicitacaoentrega> SolicitacaoentregaIdEntregadorNavigations { get; set; } = new List<Solicitacaoentrega>();

    public virtual ICollection<Solicitacaoentrega> SolicitacaoentregaIdSolicitanteNavigations { get; set; } = new List<Solicitacaoentrega>();
}
