using System;
using System.Collections.Generic;

namespace Core;

public partial class Banco
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public virtual ICollection<Dadosbancario> Dadosbancarios { get; set; } = new List<Dadosbancario>();
}
