
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;

namespace ServiceTest
{
    [TestClass]
    public class SolicitacaoEntregaServiceTests
    {
        private BiPiEntregasContext context = null!;
        private ISolicitacaoEntregaService serviceSolicitacaoEntrega = null!;

        [TestInitialize]
        public void Inicializar()
        {
            var nomeBanco = $"BiPiEntregas_{Guid.NewGuid()}";

            var opcoes =
                new DbContextOptionsBuilder<BiPiEntregasContext>()
                    .UseInMemoryDatabase(nomeBanco)
                    .Options;

            context = new BiPiEntregasContext(opcoes);

            serviceSolicitacaoEntrega = new SolicitacaoEntregaService(context);
        }

        [TestCleanup]
        public void Finalizar()
        {
            context.Dispose();
        }

        // Cria uma solicitação de entrega com dados válidos para os testes.
        private static Solicitacaoentrega NovaSolicitacao(uint id, string status = "PENDENTE")
        {
            return new Solicitacaoentrega
            {
                Id = id,
                IdSolicitante = 1,
                RuaOrigem = "Rua das Flores",
                NumeroOrigem = "120",
                BairroOrigem = "Jardins",
                CidadeOrigem = "Aracaju",
                CepOrigem = "49000000",
                RuaDestino = "Avenida Paulista",
                NumeroDestino = "1578",
                BairroDestino = "Centro",
                CidadeDestino = "Aracaju",
                CepDestino = "49000001",
                DescricaoItem = "Caixa com documentos",
                Peso = 2,
                Dimensoes = "Pequeno",
                ValorFrete = 15,
                Status = status,
                DataCriacao = new DateTime(2026, 11, 17)
            };
        }

        // ---------- GET ----------

        [TestMethod]
        public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
        {
            // Arrange
            uint id = 999;

            // Act
            var solicitacao = serviceSolicitacaoEntrega.Get(id);

            // Assert
            Assert.IsNull(solicitacao);
        }

        [TestMethod]
        public void Get_QuandoIdExiste_DeveRetornarSolicitacaoCorrespondente()
        {
            // Arrange
            context.Solicitacaoentregas.Add(NovaSolicitacao(1));
            context.SaveChanges();

            // Act
            var solicitacao = serviceSolicitacaoEntrega.Get(1);

            // Assert
            Assert.IsNotNull(solicitacao);
            Assert.AreEqual(1u, solicitacao.Id);
            Assert.AreEqual("Caixa com documentos", solicitacao.DescricaoItem);
            Assert.AreEqual("PENDENTE", solicitacao.Status);
        }

        // ---------- CREATE ----------

        [TestMethod]
        public void Create_QuandoDadosSaoValidos_DeveRetornarIdDaSolicitacaoCriada()
        {
            // Arrange
            var solicitacao = NovaSolicitacao(2);

            // Act
            var idRetornado = serviceSolicitacaoEntrega.Create(solicitacao);

            // Assert
            Assert.AreEqual(2u, idRetornado);
        }

        [TestMethod]
        public void Create_QuandoDadosSaoValidos_DeveDefinirStatusPendenteEFreteCalculado()
        {
            // Arrange
            // Status, entregador e frete enviados devem ser ignorados pelo Service.
            var solicitacao = NovaSolicitacao(3, "CONCLUIDA");
            solicitacao.IdEntregador = 10;
            solicitacao.ValorFrete = 0;

            // Act
            serviceSolicitacaoEntrega.Create(solicitacao);

            // Limpa o rastreamento para consultar os dados salvos.
            context.ChangeTracker.Clear();

            // Assert
            var solicitacaoCriada = serviceSolicitacaoEntrega.Get(3);

            Assert.IsNotNull(solicitacaoCriada);
            Assert.AreEqual("PENDENTE", solicitacaoCriada.Status);
            Assert.IsNull(solicitacaoCriada.IdEntregador);
            Assert.AreEqual(15.00m, solicitacaoCriada.ValorFrete); // 10,00 + 2 kg x 2,50
            Assert.AreEqual(DateTime.Today, solicitacaoCriada.DataCriacao.Date);
        }

        [TestMethod]
        public void Create_QuandoPesoInvalido_DeveLancarExcecaoENaoArmazenar()
        {
            // Arrange
            var solicitacao = NovaSolicitacao(4);
            solicitacao.Peso = 0;

            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => serviceSolicitacaoEntrega.Create(solicitacao));

            Assert.AreEqual(0, context.Solicitacaoentregas.Count());
        }

        // ---------- EDIT ----------

        [TestMethod]
        public void Edit_QuandoSolicitacaoPendente_DeveAtualizarDadosERecalcularFrete()
        {
            // Arrange
            var solicitacao = NovaSolicitacao(5);

            context.Solicitacaoentregas.Add(solicitacao);
            context.SaveChanges();

            solicitacao.DescricaoItem = "Notebook";
            solicitacao.RuaDestino = "Rua Nova";
            solicitacao.Peso = 4;

            // Act
            serviceSolicitacaoEntrega.Edit(solicitacao);

            // Limpa o rastreamento para consultar os dados novamente.
            context.ChangeTracker.Clear();

            // Assert
            var solicitacaoAtualizada = serviceSolicitacaoEntrega.Get(5);

            Assert.IsNotNull(solicitacaoAtualizada);
            Assert.AreEqual("Notebook", solicitacaoAtualizada.DescricaoItem);
            Assert.AreEqual("Rua Nova", solicitacaoAtualizada.RuaDestino);
            Assert.AreEqual(20.00m, solicitacaoAtualizada.ValorFrete); // 10,00 + 4 kg x 2,50

            // Verifica se os demais dados foram preservados.
            Assert.AreEqual("PENDENTE", solicitacaoAtualizada.Status);
            Assert.AreEqual(1u, solicitacaoAtualizada.IdSolicitante);
        }

        [TestMethod]
        public void Edit_QuandoSolicitacaoNaoPendente_DeveLancarExcecaoENaoAlterar()
        {
            // Arrange
            var solicitacao = NovaSolicitacao(6, "EM_ANDAMENTO");

            context.Solicitacaoentregas.Add(solicitacao);
            context.SaveChanges();

            solicitacao.DescricaoItem = "Alteração não permitida";

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(
                () => serviceSolicitacaoEntrega.Edit(solicitacao));

            // Limpa o rastreamento para consultar os dados salvos.
            context.ChangeTracker.Clear();

            var solicitacaoSalva = serviceSolicitacaoEntrega.Get(6);

            Assert.IsNotNull(solicitacaoSalva);
            Assert.AreEqual("Caixa com documentos", solicitacaoSalva.DescricaoItem);
        }

        // ---------- DELETE ----------

        [TestMethod]
        public void Delete_QuandoSolicitacaoPendente_DeveCancelarSemRemover()
        {
            // Arrange
            context.Solicitacaoentregas.Add(NovaSolicitacao(7));
            context.SaveChanges();

            // Act
            serviceSolicitacaoEntrega.Delete(7);

            // Assert
            var solicitacaoCancelada = serviceSolicitacaoEntrega.Get(7);

            Assert.IsNotNull(solicitacaoCancelada);
            Assert.AreEqual("CANCELADA", solicitacaoCancelada.Status);
            Assert.AreEqual(1, context.Solicitacaoentregas.Count());
        }

        [TestMethod]
        public void Delete_QuandoSolicitacaoNaoPendente_DeveLancarExcecaoENaoCancelar()
        {
            // Arrange
            context.Solicitacaoentregas.Add(NovaSolicitacao(8, "EM_ANDAMENTO"));
            context.SaveChanges();

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(
                () => serviceSolicitacaoEntrega.Delete(8));

            var solicitacao = serviceSolicitacaoEntrega.Get(8);

            Assert.IsNotNull(solicitacao);
            Assert.AreEqual("EM_ANDAMENTO", solicitacao.Status);
        }

        [TestMethod]
        public void Delete_QuandoSolicitacaoNaoExiste_NaoDeveAlterarOsDados()
        {
            // Arrange
            context.Solicitacaoentregas.Add(NovaSolicitacao(9));
            context.SaveChanges();

            var quantidadeInicial = context.Solicitacaoentregas.Count();

            // Act
            serviceSolicitacaoEntrega.Delete(999);

            // Assert
            var quantidadeFinal = context.Solicitacaoentregas.Count();
            var solicitacaoExistente = serviceSolicitacaoEntrega.Get(9);

            Assert.AreEqual(quantidadeInicial, quantidadeFinal);
            Assert.IsNotNull(solicitacaoExistente);
            Assert.AreEqual("PENDENTE", solicitacaoExistente.Status);
        }



        [TestMethod]
        public void GetAll_QuandoExistemSolicitacoes_DeveRetornarTodasAsSolicitacoes()
        {
            // Arrange
            context.Solicitacaoentregas.AddRange(
                NovaSolicitacao(10),
                NovaSolicitacao(11, "CONCLUIDA"));
            context.SaveChanges();

            // Act
            var solicitacoes = serviceSolicitacaoEntrega.GetAll().ToList();

            // Assert
            Assert.IsNotNull(solicitacoes);
            Assert.HasCount(2, solicitacoes);

            CollectionAssert.AreEquivalent(
                new uint[] { 10, 11 },
                solicitacoes.Select(s => s.Id).ToList());
        }

        [TestMethod]
        public void GetAll_QuandoNaoHaSolicitacoes_DeveRetornarColecaoVazia()
        {
            // Arrange
            

            // Act
            var solicitacoes = serviceSolicitacaoEntrega.GetAll().ToList();

            // Assert
            Assert.IsNotNull(solicitacoes);
            Assert.IsEmpty(solicitacoes);
        }

    

        [TestMethod]
        [DataRow(1.0, 12.50)]
        [DataRow(2.0, 15.00)]
        [DataRow(10.0, 35.00)]
        public void CalculateShipping_QuandoPesoValido_DeveRetornarValorDoFrete(
            double peso, double freteEsperado)
        {
            // Act
            var frete = serviceSolicitacaoEntrega.CalculateShipping((decimal)peso);

            // Assert
            Assert.AreEqual((decimal)freteEsperado, frete);
        }

        [TestMethod]
        [DataRow(0.0)]
        [DataRow(-1.0)]
        public void CalculateShipping_QuandoPesoMenorOuIgualAZero_DeveLancarExcecao(double peso)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => serviceSolicitacaoEntrega.CalculateShipping((decimal)peso));
        }

      
        // Ainda não implementados: dependem de novas colunas no banco de dados.

        [TestMethod]
        public void Assess_QuandoChamado_DeveLancarNotImplementedException()
        {
            // Act + Assert
            Assert.ThrowsExactly<NotImplementedException>(
                () => serviceSolicitacaoEntrega.Assess(1, 5, "Ótimo entregador"));
        }

        [TestMethod]
        public void ReportProblem_QuandoChamado_DeveLancarNotImplementedException()
        {
            // Act + Assert
            Assert.ThrowsExactly<NotImplementedException>(
                () => serviceSolicitacaoEntrega.ReportProblem(1, "Item danificado"));
        }
    }
}