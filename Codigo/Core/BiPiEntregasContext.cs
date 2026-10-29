using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core;

public partial class BiPiEntregasContext : DbContext
{
    public BiPiEntregasContext()
    {
    }

    public BiPiEntregasContext(DbContextOptions<BiPiEntregasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Banco> Bancos { get; set; }

    public virtual DbSet<Cartaocredito> Cartaocreditos { get; set; }

    public virtual DbSet<Dadosbancario> Dadosbancarios { get; set; }

    public virtual DbSet<Pagamento> Pagamentos { get; set; }

    public virtual DbSet<Pessoa> Pessoas { get; set; }

    public virtual DbSet<Solicitacaoentrega> Solicitacaoentregas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=261984M@io;database=BiPiEntregas");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Banco>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("banco");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Cartaocredito>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cartaocredito");

            entity.HasIndex(e => e.IdPessoa, "fk_CartaoCredito_Pessoa1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cvv)
                .HasMaxLength(4)
                .IsFixedLength()
                .HasColumnName("cvv");
            entity.Property(e => e.IdPessoa).HasColumnName("idPessoa");
            entity.Property(e => e.NomeCartao)
                .HasMaxLength(50)
                .HasColumnName("nomeCartao");
            entity.Property(e => e.NumeroCartao)
                .HasMaxLength(19)
                .IsFixedLength()
                .HasColumnName("numeroCartao");
            entity.Property(e => e.TipoCartao)
                .HasComment("CREDITO = C\nDEBITO = D")
                .HasColumnType("enum('C','D')")
                .HasColumnName("tipoCartao");
            entity.Property(e => e.Validade)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("validade");

            entity.HasOne(d => d.IdPessoaNavigation).WithMany(p => p.Cartaocreditos)
                .HasForeignKey(d => d.IdPessoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_CartaoCredito_Pessoa1");
        });

        modelBuilder.Entity<Dadosbancario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("dadosbancario");

            entity.HasIndex(e => e.IdBanco, "fk_DadosBancario_Banco1_idx");

            entity.HasIndex(e => e.IdPessoa, "fk_DadosBancarios_Pessoa1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Agencia)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("agencia");
            entity.Property(e => e.Conta)
                .HasMaxLength(20)
                .HasComment("CONTA CORRENTE = C\nCONTA POPANÇA =  P")
                .HasColumnName("conta");
            entity.Property(e => e.CpfTitular)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("cpfTitular");
            entity.Property(e => e.IdBanco).HasColumnName("idBanco");
            entity.Property(e => e.IdPessoa).HasColumnName("idPessoa");
            entity.Property(e => e.NomeTitular)
                .HasMaxLength(50)
                .HasColumnName("nomeTitular");
            entity.Property(e => e.TipoConta)
                .HasColumnType("enum('C','P')")
                .HasColumnName("tipoConta");

            entity.HasOne(d => d.IdBancoNavigation).WithMany(p => p.Dadosbancarios)
                .HasForeignKey(d => d.IdBanco)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DadosBancario_Banco1");

            entity.HasOne(d => d.IdPessoaNavigation).WithMany(p => p.Dadosbancarios)
                .HasForeignKey(d => d.IdPessoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DadosBancarios_Pessoa1");
        });

        modelBuilder.Entity<Pagamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pagamento");

            entity.HasIndex(e => e.IdSolicitacaoEntrega, "fk_Pagamento_SolicitacaoEntrega1_idx");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataPagamento)
                .HasColumnType("date")
                .HasColumnName("dataPagamento");
            entity.Property(e => e.IdSolicitacaoEntrega).HasColumnName("idSolicitacaoEntrega");
            entity.Property(e => e.Status)
                .HasComment("PENDENTE = P\nAPROVADO = A\nRECUSADO = R")
                .HasColumnType("enum('P','A','R')")
                .HasColumnName("status");
            entity.Property(e => e.Tipo)
                .HasComment("PIX = P\nCARTÃO DE CREDITO = A")
                .HasColumnType("enum('P','C')")
                .HasColumnName("tipo");
            entity.Property(e => e.Valor)
                .HasPrecision(10)
                .HasColumnName("valor");

            entity.HasOne(d => d.IdSolicitacaoEntregaNavigation).WithMany(p => p.Pagamentos)
                .HasForeignKey(d => d.IdSolicitacaoEntrega)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Pagamento_SolicitacaoEntrega1");
        });

        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pessoa");

            entity.HasIndex(e => e.Cpf, "cpf_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Email, "email_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cep)
                .HasMaxLength(9)
                .IsFixedLength()
                .HasColumnName("cep");
            entity.Property(e => e.Cidade)
                .HasMaxLength(30)
                .HasColumnName("cidade");
            entity.Property(e => e.Cnh)
                .HasMaxLength(15)
                .IsFixedLength()
                .HasColumnName("cnh");
            entity.Property(e => e.Complemento)
                .HasMaxLength(50)
                .HasColumnName("complemento");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("cpf");
            entity.Property(e => e.DataNascimento)
                .HasColumnType("date")
                .HasColumnName("dataNascimento");
            entity.Property(e => e.DocumentoVeiculo)
                .HasColumnType("blob")
                .HasColumnName("documentoVeiculo");
            entity.Property(e => e.Email)
                .HasMaxLength(75)
                .HasColumnName("email");
            entity.Property(e => e.FotoResidencia)
                .HasColumnType("blob")
                .HasColumnName("fotoResidencia");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
            entity.Property(e => e.Numero)
                .HasMaxLength(10)
                .HasColumnName("numero");
            entity.Property(e => e.Papel)
                .HasComment("SOLICITANTE = S\nENTREGADOR = E\nADMINISTRADOR = A")
                .HasColumnType("enum('S','E','A')")
                .HasColumnName("papel");
            entity.Property(e => e.Placa)
                .HasMaxLength(7)
                .IsFixedLength()
                .HasColumnName("placa");
            entity.Property(e => e.Rua)
                .HasMaxLength(100)
                .HasColumnName("rua");
            entity.Property(e => e.Status)
                .HasComment("A - ATIVO\nI - INATIVO\nP - PENDENTE")
                .HasColumnType("enum('A','I','P')")
                .HasColumnName("status");
            entity.Property(e => e.Telefone)
                .HasMaxLength(11)
                .HasColumnName("telefone");
            entity.Property(e => e.Veiculo)
                .HasMaxLength(45)
                .HasColumnName("veiculo");
        });

        modelBuilder.Entity<Solicitacaoentrega>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("solicitacaoentrega");

            entity.HasIndex(e => e.IdSolicitante, "fk_SolicitacaoEntrega_Pessoa1_idx");

            entity.HasIndex(e => e.IdEntregador, "fk_SolicitacaoEntrega_Pessoa2_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BairroDestino)
                .HasMaxLength(100)
                .HasColumnName("bairroDestino");
            entity.Property(e => e.BairroOrigem)
                .HasMaxLength(100)
                .HasColumnName("bairroOrigem");
            entity.Property(e => e.CepDestino)
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName("cepDestino");
            entity.Property(e => e.CepOrigem)
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName("cepOrigem");
            entity.Property(e => e.CidadeDestino)
                .HasMaxLength(100)
                .HasColumnName("cidadeDestino");
            entity.Property(e => e.CidadeOrigem)
                .HasMaxLength(100)
                .HasColumnName("cidadeOrigem");
            entity.Property(e => e.DataCriacao)
                .HasColumnType("date")
                .HasColumnName("dataCriacao");
            entity.Property(e => e.DescricaoItem)
                .HasMaxLength(255)
                .HasColumnName("descricaoItem");
            entity.Property(e => e.Dimensoes)
                .HasMaxLength(30)
                .IsFixedLength()
                .HasColumnName("dimensoes");
            entity.Property(e => e.IdEntregador).HasColumnName("idEntregador");
            entity.Property(e => e.IdSolicitante).HasColumnName("idSolicitante");
            entity.Property(e => e.NumeroDestino)
                .HasMaxLength(100)
                .HasColumnName("numeroDestino");
            entity.Property(e => e.NumeroOrigem)
                .HasMaxLength(100)
                .HasColumnName("numeroOrigem");
            entity.Property(e => e.Peso).HasColumnName("peso");
            entity.Property(e => e.RuaDestino)
                .HasMaxLength(100)
                .HasColumnName("ruaDestino");
            entity.Property(e => e.RuaOrigem)
                .HasMaxLength(100)
                .HasColumnName("ruaOrigem");
            entity.Property(e => e.Status)
                .HasColumnType("enum('PENDENTE','EM_ANDAMENTO','CONCLUIDA','CANCELADA')")
                .HasColumnName("status");
            entity.Property(e => e.ValorFrete)
                .HasPrecision(10)
                .HasColumnName("valorFrete");

            entity.HasOne(d => d.IdEntregadorNavigation).WithMany(p => p.SolicitacaoentregaIdEntregadorNavigations)
                .HasForeignKey(d => d.IdEntregador)
                .HasConstraintName("fk_SolicitacaoEntrega_Pessoa2");

            entity.HasOne(d => d.IdSolicitanteNavigation).WithMany(p => p.SolicitacaoentregaIdSolicitanteNavigations)
                .HasForeignKey(d => d.IdSolicitante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_SolicitacaoEntrega_Pessoa1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
