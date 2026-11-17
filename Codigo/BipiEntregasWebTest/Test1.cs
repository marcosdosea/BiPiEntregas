using AutoMapper;
using BipiEntregasWeb.Controllers;
using BipiEntregasWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BipiEntregasWebTest
{
    [TestClass]
    public class SolicitanteControllerTests
    {
        private Mock<IPessoaService> serviceMock = null!;
        private Mock<IMapper> mapperMock = null!;
        private SolicitanteController controller = null!;

        [TestInitialize]
        public void Inicializar()
        {
            serviceMock = new Mock<IPessoaService>();
            mapperMock = new Mock<IMapper>();
            controller = new SolicitanteController(serviceMock.Object, mapperMock.Object);
        }

        private static Pessoa NovaPessoa(uint id, string papel = "S", string status = "A")
        {
            return new Pessoa
            {
                Id = id,
                Nome = "Solicitante Teste",
                Cpf = "12345678909",
                Email = "solicitante@example.com",
                Telefone = "79999999999",
                DataNascimento = new DateTime(2000, 1, 1),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "100",
                Papel = papel,
                Status = status
            };
        }

        private static SolicitanteViewModel NovoViewModel(uint id = 0)
        {
            return new SolicitanteViewModel
            {
                Id = id,
                Nome = "Solicitante Teste",
                Cpf = "12345678909",
                Email = "solicitante@example.com",
                Telefone = "79999999999",
                DataNascimento = new DateTime(2000, 1, 1),
                Cep = "49000000",
                Cidade = "Aracaju",
                Rua = "Rua Teste",
                Numero = "100",
                Status = "A"
            };
        }

        // INDEX — 2 TESTES
        [TestMethod]
        public void Index_QuandoExistemPessoas_DeveListarSomenteSolicitantes()
        {
            // Arrange
            serviceMock.Setup(s => s.GetAll()).Returns(new List<Pessoa>
            {
                NovaPessoa(1, "S"), NovaPessoa(2, "E"), NovaPessoa(3, "S")
            });
            mapperMock.Setup(m => m.Map<List<SolicitanteViewModel>>(It.IsAny<object>()))
                .Returns(new List<SolicitanteViewModel> { NovoViewModel(1), NovoViewModel(3) });

            // Act
            var resultado = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(resultado);
            var modelo = resultado.Model as List<SolicitanteViewModel>;
            Assert.IsNotNull(modelo);
            Assert.HasCount(2, modelo);
            mapperMock.Verify(m => m.Map<List<SolicitanteViewModel>>(
                It.Is<object>(origem => ((List<Pessoa>)origem).Count == 2 &&
                                         ((List<Pessoa>)origem).All(p => p.Papel == "S"))),
                Times.Once);
        }

        [TestMethod]
        public void Index_QuandoNaoExistemPessoas_DeveRetornarListaVazia()
        {
            // Arrange
            serviceMock.Setup(s => s.GetAll()).Returns(new List<Pessoa>());
            mapperMock.Setup(m => m.Map<List<SolicitanteViewModel>>(It.IsAny<object>()))
                .Returns(new List<SolicitanteViewModel>());

            // Act
            var resultado = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(resultado);
            var modelo = resultado.Model as List<SolicitanteViewModel>;
            Assert.IsNotNull(modelo);
            Assert.IsEmpty(modelo);
        }

        // DETAILS — 2 TESTES
        [TestMethod]
        public void Details_QuandoSolicitanteExiste_DeveRetornarViewComDados()
        {
            // Arrange
            var pessoa = NovaPessoa(4);
            var viewModel = NovoViewModel(4);
            serviceMock.Setup(s => s.Get(4)).Returns(pessoa);
            mapperMock.Setup(m => m.Map<SolicitanteViewModel>(pessoa)).Returns(viewModel);

            // Act
            var resultado = controller.Details(4) as ViewResult;

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreSame(viewModel, resultado.Model);
        }

        [TestMethod]
        public void Details_QuandoSolicitanteNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            serviceMock.Setup(s => s.Get(999)).Returns((Pessoa?)null);

            // Act
            var resultado = controller.Details(999);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
        }

        // CREATE — 3 TESTES
        [TestMethod]
        public void Create_Get_DeveRetornarFormularioVazio()
        {
            // Act
            var resultado = controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(resultado);
            Assert.IsInstanceOfType<SolicitanteViewModel>(resultado.Model);
        }

        [TestMethod]
        public void Create_Post_QuandoDadosValidos_DeveCadastrarSolicitanteAtivo()
        {
            // Arrange
            var viewModel = NovoViewModel();
            serviceMock.Setup(s => s.Create(It.IsAny<Pessoa>())).Returns(20u);

            // Act
            var resultado = controller.Create(viewModel);

            // Assert
            var redirecionamento = resultado as RedirectToActionResult;
            Assert.IsNotNull(redirecionamento);
            Assert.AreEqual("Index", redirecionamento.ActionName);
            serviceMock.Verify(s => s.Create(It.Is<Pessoa>(p =>
                p.Papel == "S" && p.Status == "A" &&
                p.Nome == viewModel.Nome && p.Cpf == viewModel.Cpf &&
                p.Email == viewModel.Email)), Times.Once);
        }

        [TestMethod]
        public void Create_Post_QuandoModelStateInvalido_NaoDeveCadastrar()
        {
            // Arrange
            var viewModel = NovoViewModel();
            controller.ModelState.AddModelError("Nome", "Nome obrigatório");

            // Act
            var resultado = controller.Create(viewModel);

            // Assert
            var view = resultado as ViewResult;
            Assert.IsNotNull(view);
            Assert.AreSame(viewModel, view.Model);
            serviceMock.Verify(s => s.Create(It.IsAny<Pessoa>()), Times.Never);
        }

        // EDIT — 4 TESTES
        [TestMethod]
        public void Edit_Get_QuandoSolicitanteExiste_DeveAbrirFormulario()
        {
            // Arrange
            var pessoa = NovaPessoa(5);
            var viewModel = NovoViewModel(5);
            serviceMock.Setup(s => s.Get(5)).Returns(pessoa);
            mapperMock.Setup(m => m.Map<SolicitanteViewModel>(pessoa)).Returns(viewModel);

            // Act
            var resultado = controller.Edit(5) as ViewResult;

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreSame(viewModel, resultado.Model);
        }

        [TestMethod]
        public void Edit_Get_QuandoPessoaNaoEhSolicitante_DeveRetornarNotFound()
        {
            // Arrange
            serviceMock.Setup(s => s.Get(6)).Returns(NovaPessoa(6, "E"));

            // Act
            var resultado = controller.Edit(6);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
        }

        [TestMethod]
        public void Edit_Post_QuandoDadosValidos_DeveAtualizarEPreservarCamposRestritos()
        {
            // Arrange
            var pessoa = NovaPessoa(7, "S", "I");
            serviceMock.Setup(s => s.Get(7)).Returns(pessoa);
            var viewModel = NovoViewModel(7);
            viewModel.Nome = "Nome Atualizado";
            viewModel.Email = "novo@example.com";
            viewModel.Cpf = "99999999999"; // Tentativa de alterar o CPF.
            viewModel.Status = "A";         // Tentativa de alterar o Status.

            // Act
            var resultado = controller.Edit(7, viewModel);

            // Assert
            var redirecionamento = resultado as RedirectToActionResult;
            Assert.IsNotNull(redirecionamento);
            Assert.AreEqual("Index", redirecionamento.ActionName);
            serviceMock.Verify(s => s.Edit(It.Is<Pessoa>(p =>
                p.Id == 7 && p.Nome == "Nome Atualizado" &&
                p.Email == "novo@example.com" &&
                p.Cpf == "12345678909" && p.Papel == "S" && p.Status == "I")),
                Times.Once);
        }

        [TestMethod]
        public void Edit_Post_QuandoIdDiferente_DeveRetornarBadRequest()
        {
            // Arrange
            var viewModel = NovoViewModel(8);

            // Act
            var resultado = controller.Edit(9, viewModel);

            // Assert
            Assert.IsInstanceOfType<BadRequestResult>(resultado);
            serviceMock.Verify(s => s.Edit(It.IsAny<Pessoa>()), Times.Never);
        }

        // DELETE — 4 TESTES (DESATIVAÇÃO LÓGICA)
        [TestMethod]
        public void Delete_Get_QuandoSolicitanteExiste_DeveAbrirConfirmacao()
        {
            // Arrange
            var pessoa = NovaPessoa(10);
            var viewModel = NovoViewModel(10);
            serviceMock.Setup(s => s.Get(10)).Returns(pessoa);
            mapperMock.Setup(m => m.Map<SolicitanteViewModel>(pessoa)).Returns(viewModel);

            // Act
            var resultado = controller.Delete(10) as ViewResult;

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreSame(viewModel, resultado.Model);
        }

        [TestMethod]
        public void Delete_Get_QuandoPessoaNaoEhSolicitante_DeveRetornarNotFound()
        {
            // Arrange
            serviceMock.Setup(s => s.Get(11)).Returns(NovaPessoa(11, "E"));

            // Act
            var resultado = controller.Delete(11);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
        }

        [TestMethod]
        public void Delete_Post_QuandoSolicitanteExiste_DeveSolicitarDesativacao()
        {
            // Arrange
            serviceMock.Setup(s => s.Get(12)).Returns(NovaPessoa(12, "S"));

            // Act
            // O parâmetro IFormCollection não é utilizado pelo método atual.
            var resultado = controller.Delete(12, null!);

            // Assert
            var redirecionamento = resultado as RedirectToActionResult;
            Assert.IsNotNull(redirecionamento);
            Assert.AreEqual("Index", redirecionamento.ActionName);
            serviceMock.Verify(s => s.Delete(12), Times.Once);
        }

        [TestMethod]
        public void Delete_Post_QuandoSolicitanteNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            serviceMock.Setup(s => s.Get(999)).Returns((Pessoa?)null);

            // Act
            var resultado = controller.Delete(999, null!);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
            serviceMock.Verify(s => s.Delete(It.IsAny<uint>()), Times.Never);
        }
    }

}
