
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;

namespace ServiceTest
{
    [TestClass]
    public class PessoaServiceTests
    {
        private BiPiEntregasContext context = null!;
        private IPessoaService servicePessoa = null!;

        [TestInitialize]
        public void Inicializar()
        {
            var nomeBanco = $"BiPiEntregas_{Guid.NewGuid()}";

            var opcoes =
                new DbContextOptionsBuilder<BiPiEntregasContext>()
                    .UseInMemoryDatabase(nomeBanco)
                    .Options;

            context = new BiPiEntregasContext(opcoes);

            servicePessoa = new PessoaService(context);
        }

        [TestCleanup]
        public void Finalizar()
        {
            context.Dispose();
        }

        [TestMethod]
        public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
        {
            // Arrange
            uint id = 999;

            // Act
            var pessoa = servicePessoa.Get(id);

            // Assert
            Assert.IsNull(pessoa);
        }


        [TestMethod]
        public void Get_QuandoIdExiste_DeveRetornarPessoaCorrespondente()
        {
            // Arrange
            var pessoaEsperada = new Pessoa
            {
                Id = 1,
                Nome = "Pessoa Teste",
                Cpf = "12345678909",
                Email = "pessoa.teste@example.com",
                Telefone = "79999999999",
                DataNascimento = new DateTime(2000, 1, 1),
                Cep = "49000000",
                Cidade = "Cidade Teste",
                Rua = "Rua Teste",
                Numero = "10",
                Papel = "S",
                Status = "A"
            };

            context.Pessoas.Add(pessoaEsperada);
            context.SaveChanges();

            // Act
            var pessoa = servicePessoa.Get(1);

            // Assert
            Assert.IsNotNull(pessoa);
            Assert.AreEqual(1u, pessoa.Id);
            Assert.AreEqual("Pessoa Teste", pessoa.Nome);
            Assert.AreEqual("S", pessoa.Papel);
            Assert.AreEqual("A", pessoa.Status);
        }


        [TestMethod]
        public void Create_QuandoDadosSaoValidos_DeveRetornarIdDaPessoaCriada()
        {
            // Arrange
            var pessoa = new Pessoa
            {
                Id = 4,
                Nome = "Solicitante Teste",
                Cpf = "12345678909",
                Email = "solicitante@example.com",
                Telefone = "79999999999",
                DataNascimento = new DateTime(2000, 5, 15),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "100",
                Papel = "S",
                Status = "A"
            };

            // Act
            var idRetornado = servicePessoa.Create(pessoa);

            // Assert
            Assert.AreEqual(4u, idRetornado);
        }


        [TestMethod]
        public void Create_QuandoDadosSaoValidos_DeveArmazenarPessoa()
        {
            // Arrange
            var pessoaEsperada = new Pessoa
            {
                Id = 5,
                Nome = "Solicitante Teste",
                Cpf = "98765432100",
                Email = "teste.criacao@example.com",
                Telefone = "79999999999",
                DataNascimento = new DateTime(2001, 6, 10),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "100",
                Papel = "S",
                Status = "A"
            };

            // Act
            servicePessoa.Create(pessoaEsperada);

            // Assert
            var pessoaCriada = servicePessoa.Get(5);

            Assert.IsNotNull(pessoaCriada);
            Assert.AreEqual(pessoaEsperada.Id, pessoaCriada.Id);
            Assert.AreEqual(pessoaEsperada.Nome, pessoaCriada.Nome);
            Assert.AreEqual(pessoaEsperada.Cpf, pessoaCriada.Cpf);
            Assert.AreEqual(pessoaEsperada.Email, pessoaCriada.Email);
            Assert.AreEqual("S", pessoaCriada.Papel);
            Assert.AreEqual("A", pessoaCriada.Status);
        }


        [TestMethod]
        public void Delete_QuandoPessoaExiste_DeveDesativarSemRemover()
        {
            // Arrange
            var pessoa = new Pessoa
            {
                Id = 6,
                Nome = "Solicitante Teste",
                Cpf = "12345678909",
                Email = "teste.desativacao@example.com",
                Telefone = "79999999999",
                DataNascimento = new DateTime(2000, 1, 1),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "100",
                Papel = "S",
                Status = "A"
            };

            context.Pessoas.Add(pessoa);
            context.SaveChanges();

            // Act
            servicePessoa.Delete(6);

            // Assert
            var pessoaDesativada = servicePessoa.Get(6);

            Assert.IsNotNull(pessoaDesativada);
            Assert.AreEqual(6u, pessoaDesativada.Id);
            Assert.AreEqual("I", pessoaDesativada.Status);
            Assert.AreEqual("S", pessoaDesativada.Papel);
            Assert.AreEqual(1, context.Pessoas.Count());
        }

        [TestMethod]
        public void Delete_QuandoPessoaNaoExiste_NaoDeveAlterarOsDados()
        {
            // Arrange
            var pessoa = new Pessoa
            {
                Id = 7,
                Nome = "Solicitante Teste",
                Cpf = "12345678909",
                Email = "teste.inexistente@example.com",
                Telefone = "79999999999",
                DataNascimento = new DateTime(2000, 1, 1),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "100",
                Papel = "S",
                Status = "A"
            };

            context.Pessoas.Add(pessoa);
            context.SaveChanges();

            var quantidadeInicial = context.Pessoas.Count();

            // Act
            servicePessoa.Delete(999);

            // Assert
            var quantidadeFinal = context.Pessoas.Count();
            var pessoaExistente = servicePessoa.Get(7);

            Assert.AreEqual(quantidadeInicial, quantidadeFinal);
            Assert.IsNotNull(pessoaExistente);
            Assert.AreEqual("A", pessoaExistente.Status);
        }

        [TestMethod]
        public void Edit_QuandoPessoaExiste_DeveAtualizarSeusDados()
        {
            // Arrange
            var pessoa = new Pessoa
            {
                Id = 8,
                Nome = "Solicitante Teste",
                Cpf = "12345678909",
                Email = "teste.edicao@example.com",
                Telefone = "79999999999",
                DataNascimento = new DateTime(2000, 1, 1),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "100",
                Papel = "S",
                Status = "A"
            };

            context.Pessoas.Add(pessoa);
            context.SaveChanges();

            pessoa.Nome = "Solicitante Atualizado";
            pessoa.Email = "atualizado@example.com";
            pessoa.Telefone = "79888888888";

            // Act
            servicePessoa.Edit(pessoa);

            // Limpa o rastreamento para consultar os dados novamente.
            context.ChangeTracker.Clear();

            // Assert
            var pessoaAtualizada = servicePessoa.Get(8);

            Assert.IsNotNull(pessoaAtualizada);
            Assert.AreEqual("Solicitante Atualizado", pessoaAtualizada.Nome);
            Assert.AreEqual("atualizado@example.com", pessoaAtualizada.Email);
            Assert.AreEqual("79888888888", pessoaAtualizada.Telefone);

            // Verifica se os demais dados foram preservados.
            Assert.AreEqual("12345678909", pessoaAtualizada.Cpf);
            Assert.AreEqual("S", pessoaAtualizada.Papel);
            Assert.AreEqual("A", pessoaAtualizada.Status);
        }

        [TestMethod]
        public void GetAll_QuandoExistemPessoas_DeveRetornarTodasAsPessoas()
        {
            // Arrange
            var pessoa1 = new Pessoa
            {
                Id = 9,
                Nome = "Solicitante Teste",
                Cpf = "12345678909",
                Email = "solicitante@example.com",
                Telefone = "79999999999",
                DataNascimento = new DateTime(2000, 1, 1),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "100",
                Papel = "S",
                Status = "A"
            };

            var pessoa2 = new Pessoa
            {
                Id = 10,
                Nome = "Entregador Teste",
                Cpf = "98765432100",
                Email = "entregador@example.com",
                Telefone = "79888888888",
                DataNascimento = new DateTime(1998, 5, 10),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "200",
                Papel = "E",
                Status = "A"
            };

            context.Pessoas.AddRange(pessoa1, pessoa2);
            context.SaveChanges();

            // Act
            var pessoas = servicePessoa.GetAll().ToList();

            // Assert
            Assert.IsNotNull(pessoas);
            Assert.HasCount(2, pessoas);

            CollectionAssert.AreEquivalent(
                new uint[] { 9, 10 },
                pessoas.Select(p => p.Id).ToList());
        }

        [TestMethod]
        public void GetAll_QuandoNaoHaPessoas_DeveRetornarColecaoVazia()
        {
            // Arrange
            // O banco em memória começa vazio.
            // Garantimos que não existam pessoas cadastradas.
            context.Pessoas.RemoveRange(context.Pessoas);
            context.SaveChanges();

            // Act
            var pessoas = servicePessoa.GetAll().ToList();

            // Assert
            Assert.IsNotNull(pessoas);
            Assert.IsEmpty(pessoas);
        }

        [TestMethod]
        public void GetAll_QuandoStatusEhAtivo_DeveRetornarSomentePessoasAtivas()
        {
            // Arrange
            var pessoa1 = new Pessoa
            {
                Id = 11,
                Nome = "Solicitante Ativo",
                Cpf = "12345678909",
                Email = "ativo@example.com",
                Telefone = "79999999999",
                DataNascimento = new DateTime(2000, 1, 1),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "100",
                Papel = "S",
                Status = "A"
            };

            var pessoa2 = new Pessoa
            {
                Id = 12,
                Nome = "Solicitante Inativo",
                Cpf = "98765432100",
                Email = "inativo@example.com",
                Telefone = "79888888888",
                DataNascimento = new DateTime(2001, 2, 2),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "200",
                Papel = "S",
                Status = "I"
            };

            var pessoa3 = new Pessoa
            {
                Id = 13,
                Nome = "Solicitante Pendente",
                Cpf = "11144477735",
                Email = "pendente@example.com",
                Telefone = "79777777777",
                DataNascimento = new DateTime(2002, 3, 3),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "300",
                Papel = "S",
                Status = "P"
            };

            context.Pessoas.AddRange(pessoa1, pessoa2, pessoa3);
            context.SaveChanges();

            // Act
            var pessoasAtivas = servicePessoa.GetAll("A").ToList();

            // Assert
            Assert.IsNotNull(pessoasAtivas);
            Assert.HasCount(1, pessoasAtivas);

            Assert.AreEqual(11u, pessoasAtivas[0].Id);
            Assert.AreEqual("A", pessoasAtivas[0].Status);

            Assert.IsTrue(pessoasAtivas.All(p => p.Status == "A"));
        }

        [TestMethod]
        public void GetAll_QuandoStatusEhInativo_DeveRetornarSomentePessoasInativas()
        {
            // Arrange
            var pessoa1 = new Pessoa
            {
                Id = 14,
                Nome = "Solicitante Ativo",
                Cpf = "12345678909",
                Email = "ativo14@example.com",
                Telefone = "79999999999",
                DataNascimento = new DateTime(2000, 1, 1),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "100",
                Papel = "S",
                Status = "A"
            };

            var pessoa2 = new Pessoa
            {
                Id = 15,
                Nome = "Solicitante Inativo",
                Cpf = "98765432100",
                Email = "inativo15@example.com",
                Telefone = "79888888888",
                DataNascimento = new DateTime(2001, 2, 2),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "200",
                Papel = "S",
                Status = "I"
            };

            context.Pessoas.AddRange(pessoa1, pessoa2);
            context.SaveChanges();

            // Act
            var pessoasInativas = servicePessoa.GetAll("I").ToList();

            // Assert
            Assert.IsNotNull(pessoasInativas);
            Assert.HasCount(1, pessoasInativas);
            Assert.AreEqual(15u, pessoasInativas[0].Id);
            Assert.AreEqual("I", pessoasInativas[0].Status);

            Assert.IsTrue(pessoasInativas.All(p => p.Status == "I"));
        }

        [TestMethod]
        public void GetAll_QuandoStatusEhPendente_DeveRetornarSomentePessoasPendentes()
        {
            // Arrange
            var pessoa1 = new Pessoa
            {
                Id = 16,
                Nome = "Solicitante Ativo",
                Cpf = "12345678909",
                Email = "ativo16@example.com",
                Telefone = "79999999999",
                DataNascimento = new DateTime(2000, 1, 1),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "100",
                Papel = "S",
                Status = "A"
            };

            var pessoa2 = new Pessoa
            {
                Id = 17,
                Nome = "Solicitante Pendente",
                Cpf = "98765432100",
                Email = "pendente17@example.com",
                Telefone = "79888888888",
                DataNascimento = new DateTime(2001, 2, 2),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "200",
                Papel = "S",
                Status = "P"
            };

            var pessoa3 = new Pessoa
            {
                Id = 18,
                Nome = "Solicitante Inativo",
                Cpf = "11144477735",
                Email = "inativo18@example.com",
                Telefone = "79777777777",
                DataNascimento = new DateTime(2002, 3, 3),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "300",
                Papel = "S",
                Status = "I"
            };

            context.Pessoas.AddRange(pessoa1, pessoa2, pessoa3);
            context.SaveChanges();

            // Act
            var pessoasPendentes = servicePessoa.GetAll("P").ToList();

            // Assert
            Assert.IsNotNull(pessoasPendentes);
            Assert.HasCount(1, pessoasPendentes);

            Assert.AreEqual(17u, pessoasPendentes[0].Id);
            Assert.AreEqual("P", pessoasPendentes[0].Status);

            Assert.IsTrue(pessoasPendentes.All(p => p.Status == "P"));
        }

        [TestMethod]
        public void Activate_QuandoPessoaEstaInativa_DeveReativar()
        {
            // Arrange
            var pessoa = new Pessoa
            {
                Id = 19,
                Nome = "Solicitante Inativo",
                Cpf = "12345678909",
                Email = "reativacao@example.com",
                Telefone = "79999999999",
                DataNascimento = new DateTime(2000, 1, 1),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "100",
                Papel = "S",
                Status = "I"
            };

            context.Pessoas.Add(pessoa);
            context.SaveChanges();

            // Act
            servicePessoa.Activate(19);

            // Limpa o rastreamento para consultar os dados salvos.
            context.ChangeTracker.Clear();

            // Assert
            var pessoaReativada = servicePessoa.Get(19);

            Assert.IsNotNull(pessoaReativada);
            Assert.AreEqual(19u, pessoaReativada.Id);
            Assert.AreEqual("A", pessoaReativada.Status);
            Assert.AreEqual("S", pessoaReativada.Papel);

            // Confirma que o registro continua existindo.
            Assert.AreEqual(1, context.Pessoas.Count());
        }

    }
}
