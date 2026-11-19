namespace Core;

public partial class PessoaPerfil
{
    public uint IdPessoa { get; set; }
       public string Papel { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Pessoa Pessoa { get; set; } = null!;
}