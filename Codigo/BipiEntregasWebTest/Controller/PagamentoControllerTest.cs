
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Core;
using Core.Service;
using BipiEntregasWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BipiEntregasWeb.Controllers.Tests;

[TestClass]
public class PagamentoControllerTests
{
    private Mock<IPagamentoService> pagamentoServiceMock = null!;
    private Mock<IMapper> mapperMock = null!;
    private PagamentoController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        pagamentoServiceMock = new Mock<IPagamentoService>();
        mapperMock = new Mock<IMapper>();

        pagamentoServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterPagamentosDeTeste());

        pagamentoServiceMock
            .Setup(service => service.Get(1))
            .Returns(ObterPagamentoAlvo());

        pagamentoServiceMock
            .Setup(service => service.Get(999))
            .Returns((Pagamento?)null);

        mapperMock
            .Setup(mapper => mapper.Map<List<PagamentoViewModel>>(
                It.IsAny<object>()))
            .Returns((object origem) =>
            {
                var pagamentos = (IEnumerable<Pagamento>)origem;

                return pagamentos.Select(MapearPagamento).ToList();
            });

        mapperMock
            .Setup(mapper => mapper.Map<PagamentoViewModel>(
                It.IsAny<object>()))
            .Returns((object origem) =>
                MapearPagamento((Pagamento)origem));

        controller = new PagamentoController(
            pagamentoServiceMock.Object,
            mapperMock.Object);
    }

    // =========================================================
    // INDEX
    // =========================================================

    [TestMethod]
    public void Index_QuandoChamado_DeveRetornarViewComTodosOsPagamentos()
    {
        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var pagamentos = Assert.IsInstanceOfType<List<PagamentoViewModel>>(
            viewResult.ViewData.Model);

        Assert.AreEqual(2, pagamentos.Count);

        Assert.IsTrue(pagamentos.All(pagamento =>
            pagamento.Valor > 0));

        pagamentoServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    [TestMethod]
    public void Index_QuandoNaoExistemPagamentos_DeveRetornarViewComColecaoVazia()
    {
        // Arrange
        pagamentoServiceMock
            .Setup(service => service.GetAll())
            .Returns(Array.Empty<Pagamento>());

        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var pagamentos = Assert.IsInstanceOfType<List<PagamentoViewModel>>(
            viewResult.ViewData.Model);

        Assert.IsEmpty(pagamentos);

        pagamentoServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    // =========================================================
    // DETAILS
    // =========================================================

    [TestMethod]
    public void Details_QuandoPagamentoExiste_DeveRetornarViewComPagamento()
    {
        // Act
        var resultado = controller.Details(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var pagamento = Assert.IsInstanceOfType<PagamentoViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, pagamento.Id);
        Assert.AreEqual(10u, pagamento.IdSolicitacaoEntrega);
        Assert.AreEqual(32.30m, pagamento.Valor);
        Assert.AreEqual("C", pagamento.Tipo);
        Assert.AreEqual("P", pagamento.Status);

        pagamentoServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Details_QuandoPagamentoNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Details(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pagamentoServiceMock.Verify(
            service => service.Get(999),
            Times.Once);

        mapperMock.Verify(
            mapper => mapper.Map<PagamentoViewModel>(
                It.IsAny<object>()),
            Times.Never);
    }

    // =========================================================
    // CREATE - GET
    // =========================================================

    [TestMethod]
    public void Create_QuandoRequisicaoGet_DeveRetornarViewComModeloInicial()
    {
        // Act
        var resultado = controller.Create();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var pagamento = Assert.IsInstanceOfType<PagamentoViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(0u, pagamento.Id);
        Assert.AreEqual(0u, pagamento.IdSolicitacaoEntrega);
        Assert.AreEqual(0m, pagamento.Valor);
        Assert.IsTrue(string.IsNullOrEmpty(pagamento.Tipo));
    }

    // =========================================================
    // CREATE - POST
    // =========================================================

    [TestMethod]
    public void Create_QuandoModeloValido_DeveCriarPagamentoERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeNovoPagamento();
        ValidarModelo(modelo);

        Pagamento? pagamentoCriado = null;

        pagamentoServiceMock
            .Setup(service => service.Create(It.IsAny<Pagamento>()))
            .Callback<Pagamento>(pagamento =>
                pagamentoCriado = pagamento)
            .Returns(10u);

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(pagamentoCriado);

        Assert.AreEqual(
            modelo.IdSolicitacaoEntrega,
            pagamentoCriado.IdSolicitacaoEntrega);
        Assert.AreEqual(modelo.Valor, pagamentoCriado.Valor);
        Assert.AreEqual(modelo.Tipo, pagamentoCriado.Tipo);

        pagamentoServiceMock.Verify(
            service => service.Create(It.IsAny<Pagamento>()),
            Times.Once);
    }

    [TestMethod]
    public void Create_QuandoModeloInvalido_NaoDeveCriarPagamento()
    {
        // Arrange
        var modelo = CriarModeloDeNovoPagamento();

        controller.ModelState.AddModelError(
            nameof(PagamentoViewModel.Valor),
            "Informe um valor maior que zero.");

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        pagamentoServiceMock.Verify(
            service => service.Create(It.IsAny<Pagamento>()),
            Times.Never);
    }

    [TestMethod]
    public void Create_QuandoValorForZero_NaoDeveCriarPagamento()
    {
        // Arrange
        var modelo = CriarModeloDeNovoPagamento();
        modelo.Valor = 0;

        ValidarModelo(modelo);

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(
            nameof(PagamentoViewModel.Valor)));

        pagamentoServiceMock.Verify(
            service => service.Create(It.IsAny<Pagamento>()),
            Times.Never);
    }

    [TestMethod]
    public void Create_QuandoFormaDePagamentoForInvalida_NaoDeveCriarPagamento()
    {
        // Arrange
        var modelo = CriarModeloDeNovoPagamento();
        modelo.Tipo = "X";

        ValidarModelo(modelo);

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(
            nameof(PagamentoViewModel.Tipo)));

        pagamentoServiceMock.Verify(
            service => service.Create(It.IsAny<Pagamento>()),
            Times.Never);
    }

    [TestMethod]
    public void Create_QuandoServiceLancarArgumentException_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDeNovoPagamento();
        ValidarModelo(modelo);

        pagamentoServiceMock
            .Setup(service => service.Create(It.IsAny<Pagamento>()))
            .Throws(new ArgumentException(
                "O valor do pagamento deve ser maior que zero."));

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));
    }

    [TestMethod]
    public void Create_QuandoOcorrerErroNoBanco_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDeNovoPagamento();
        ValidarModelo(modelo);

        pagamentoServiceMock
            .Setup(service => service.Create(It.IsAny<Pagamento>()))
            .Throws(new DbUpdateException("Erro ao salvar"));

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));

        pagamentoServiceMock.Verify(
            service => service.Create(It.IsAny<Pagamento>()),
            Times.Once);
    }

    // =========================================================
    // EDIT - GET
    // =========================================================

    [TestMethod]
    public void Edit_QuandoPagamentoExiste_DeveRetornarViewComPagamento()
    {
        // Act
        var resultado = controller.Edit(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var pagamento = Assert.IsInstanceOfType<PagamentoViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, pagamento.Id);
        Assert.AreEqual(32.30m, pagamento.Valor);

        pagamentoServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoPagamentoNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Edit(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pagamentoServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    // =========================================================
    // EDIT - POST
    // =========================================================

    [TestMethod]
    public void Edit_QuandoModeloValido_DeveAtualizarPagamentoERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDePagamentoExistente();
        ValidarModelo(modelo);

        Pagamento? pagamentoEditado = null;

        pagamentoServiceMock
            .Setup(service => service.Edit(It.IsAny<Pagamento>()))
            .Callback<Pagamento>(pagamento =>
                pagamentoEditado = pagamento);

        // Act
        var resultado = controller.Edit(modelo.Id, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(pagamentoEditado);

        Assert.AreEqual(modelo.Id, pagamentoEditado.Id);
        Assert.AreEqual(modelo.Valor, pagamentoEditado.Valor);
        Assert.AreEqual(modelo.Tipo, pagamentoEditado.Tipo);

        // A solicitação de entrega não é alterada na edição.
        Assert.AreEqual(10u, pagamentoEditado.IdSolicitacaoEntrega);

        pagamentoServiceMock.Verify(
            service => service.Get(modelo.Id),
            Times.Once);

        pagamentoServiceMock.Verify(
            service => service.Edit(It.IsAny<Pagamento>()),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoIdsForemDiferentes_DeveRetornarBadRequest()
    {
        // Arrange
        var modelo = CriarModeloDePagamentoExistente();

        // Act
        var resultado = controller.Edit(999, modelo);

        // Assert
        Assert.IsInstanceOfType<BadRequestResult>(resultado);

        pagamentoServiceMock.Verify(
            service => service.Get(It.IsAny<uint>()),
            Times.Never);

        pagamentoServiceMock.Verify(
            service => service.Edit(It.IsAny<Pagamento>()),
            Times.Never);
    }

    [TestMethod]
    public void Edit_QuandoModeloInvalido_NaoDeveAtualizarPagamento()
    {
        // Arrange
        var modelo = CriarModeloDePagamentoExistente();

        controller.ModelState.AddModelError(
            nameof(PagamentoViewModel.Valor),
            "Informe um valor maior que zero.");

        // Act
        var resultado = controller.Edit(modelo.Id, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        pagamentoServiceMock.Verify(
            service => service.Get(It.IsAny<uint>()),
            Times.Never);

        pagamentoServiceMock.Verify(
            service => service.Edit(It.IsAny<Pagamento>()),
            Times.Never);
    }

    [TestMethod]
    public void Edit_Post_QuandoPagamentoNaoExiste_DeveRetornarNotFound()
    {
        // Arrange
        var modelo = CriarModeloDePagamentoExistente();
        modelo.Id = 999;

        ValidarModelo(modelo);

        // Act
        var resultado = controller.Edit(999, modelo);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pagamentoServiceMock.Verify(
            service => service.Get(999),
            Times.Once);

        pagamentoServiceMock.Verify(
            service => service.Edit(It.IsAny<Pagamento>()),
            Times.Never);
    }

    [TestMethod]
    public void Edit_QuandoPagamentoNaoEstiverPendente_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDePagamentoExistente();
        ValidarModelo(modelo);

        pagamentoServiceMock
            .Setup(service => service.Edit(It.IsAny<Pagamento>()))
            .Throws(new InvalidOperationException(
                "Somente pagamentos pendentes podem ser alterados."));

        // Act
        var resultado = controller.Edit(modelo.Id, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));

        pagamentoServiceMock.Verify(
            service => service.Edit(It.IsAny<Pagamento>()),
            Times.Once);
    }

    // =========================================================
    // DELETE - GET
    // =========================================================

    [TestMethod]
    public void Delete_QuandoPagamentoExiste_DeveRetornarViewComPagamento()
    {
        // Act
        var resultado = controller.Delete(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var pagamento = Assert.IsInstanceOfType<PagamentoViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, pagamento.Id);
        Assert.AreEqual(32.30m, pagamento.Valor);

        pagamentoServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoPagamentoNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Delete(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pagamentoServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    // =========================================================
    // DELETE - POST (estorno)
    // =========================================================

    [TestMethod]
    public void Delete_QuandoPagamentoExiste_DeveEstornarERedirecionarParaIndex()
    {
        // Arrange
        var collectionMock = new Mock<IFormCollection>();

        // Act
        var resultado = controller.Delete(1, collectionMock.Object);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        pagamentoServiceMock.Verify(
            service => service.Get(1),
            Times.Once);

        pagamentoServiceMock.Verify(
            service => service.Delete(1),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoPagamentoNaoExiste_NaoDeveEstornar()
    {
        // Arrange
        var collectionMock = new Mock<IFormCollection>();

        // Act
        var resultado = controller.Delete(999, collectionMock.Object);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pagamentoServiceMock.Verify(
            service => service.Delete(It.IsAny<uint>()),
            Times.Never);
    }

    [TestMethod]
    public void Delete_QuandoEstornoNaoForPermitido_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var collectionMock = new Mock<IFormCollection>();

        pagamentoServiceMock
            .Setup(service => service.Delete(1))
            .Throws(new InvalidOperationException(
                "Somente pagamentos pendentes ou aprovados podem ser estornados."));

        // Act
        var resultado = controller.Delete(1, collectionMock.Object);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var pagamento = Assert.IsInstanceOfType<PagamentoViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, pagamento.Id);

        Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));

        pagamentoServiceMock.Verify(
            service => service.Delete(1),
            Times.Once);
    }

    // =========================================================
    // MÉTODOS AUXILIARES
    // =========================================================

    private static void VerificarRedirecionamentoParaIndex(
        ActionResult resultado)
    {
        var redirectResult =
            Assert.IsInstanceOfType<RedirectToActionResult>(resultado);

        Assert.IsNull(redirectResult.ControllerName);

        Assert.AreEqual(
            nameof(PagamentoController.Index),
            redirectResult.ActionName);
    }

    private static PagamentoViewModel CriarModeloDeNovoPagamento()
    {
        return new PagamentoViewModel
        {
            IdSolicitacaoEntrega = 20,
            Valor = 45.50m,
            Tipo = "P",
            Status = "P"
        };
    }

    private static PagamentoViewModel CriarModeloDePagamentoExistente()
    {
        return new PagamentoViewModel
        {
            Id = 1,
            IdSolicitacaoEntrega = 10,
            Valor = 50.00m,
            Tipo = "P",
            Status = "P"
        };
    }

    private static Pagamento ObterPagamentoAlvo()
    {
        return new Pagamento
        {
            Id = 1,
            IdSolicitacaoEntrega = 10,
            Valor = 32.30m,
            Tipo = "C",
            Status = "P"
        };
    }

    private static IEnumerable<Pagamento> ObterPagamentosDeTeste()
    {
        return
        [
            ObterPagamentoAlvo(),
            new Pagamento
            {
                Id = 2,
                IdSolicitacaoEntrega = 11,
                Valor = 18.90m,
                Tipo = "P",
                Status = "A",
                DataPagamento = new DateTime(2026, 11, 18, 10, 30, 0)
            }
        ];
    }

    private static PagamentoViewModel MapearPagamento(Pagamento pagamento)
    {
        return new PagamentoViewModel
        {
            Id = pagamento.Id,
            IdSolicitacaoEntrega = pagamento.IdSolicitacaoEntrega,
            Valor = pagamento.Valor,
            Tipo = pagamento.Tipo,
            Status = pagamento.Status,
            DataPagamento = pagamento.DataPagamento,
            DataEstorno = pagamento.DataEstorno
        };
    }

    private void ValidarModelo(PagamentoViewModel modelo)
    {
        var resultados = new List<ValidationResult>();

        var contexto = new ValidationContext(modelo);

        Validator.TryValidateObject(
            modelo,
            contexto,
            resultados,
            validateAllProperties: true);

        foreach (var resultado in resultados)
        {
            var membros = resultado.MemberNames.Any()
                ? resultado.MemberNames
                : [string.Empty];

            foreach (var membro in membros)
            {
                controller.ModelState.AddModelError(
                    membro,
                    resultado.ErrorMessage ?? "Campo inválido.");
            }
        }
    }
}