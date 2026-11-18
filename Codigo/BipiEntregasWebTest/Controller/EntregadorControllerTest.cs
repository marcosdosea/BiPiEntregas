
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
public class EntregadorControllerTests
{
    private Mock<IPessoaService> pessoaServiceMock = null!;
    private Mock<IMapper> mapperMock = null!;
    private EntregadorController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        pessoaServiceMock = new Mock<IPessoaService>();
        mapperMock = new Mock<IMapper>();

        pessoaServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterPessoasDeTeste());

        pessoaServiceMock
            .Setup(service => service.Get(1))
            .Returns(ObterEntregadorAlvo());

        pessoaServiceMock
            .Setup(service => service.Get(999))
            .Returns((Pessoa?)null);

        pessoaServiceMock
            .Setup(service => service.Get(2))
            .Returns(ObterSolicitante());

        mapperMock
            .Setup(mapper => mapper.Map<List<EntregadorViewModel>>(
                It.IsAny<object>()))
            .Returns((object origem) =>
            {
                var pessoas = (List<Pessoa>)origem;

                return pessoas.Select(MapearEntregador).ToList();
            });

        mapperMock
            .Setup(mapper => mapper.Map<EntregadorViewModel>(
                It.IsAny<object>()))
            .Returns((object origem) =>
                MapearEntregador((Pessoa)origem));

        controller = new EntregadorController(
            pessoaServiceMock.Object,
            mapperMock.Object);
    }

    // =========================================================
    // INDEX
    // =========================================================

    [TestMethod]
    public void Index_QuandoChamado_DeveRetornarViewComTodosOsEntregadores()
    {
        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var entregadores = Assert.IsInstanceOfType<List<EntregadorViewModel>>(
            viewResult.ViewData.Model);

        Assert.AreEqual(2, entregadores.Count);

        Assert.IsTrue(entregadores.All(entregador =>
            !string.IsNullOrEmpty(entregador.Nome)));

        Assert.IsTrue(entregadores.All(entregador =>
            entregador.Id != 2));

        pessoaServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    [TestMethod]
    public void Index_QuandoNaoExistemEntregadores_DeveRetornarViewComColecaoVazia()
    {
        // Arrange
        pessoaServiceMock
            .Setup(service => service.GetAll())
            .Returns(Array.Empty<Pessoa>());

        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var entregadores = Assert.IsInstanceOfType<List<EntregadorViewModel>>(
            viewResult.ViewData.Model);

        Assert.IsEmpty(entregadores);

        pessoaServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    // =========================================================
    // DETAILS
    // =========================================================

    [TestMethod]
    public void Details_QuandoEntregadorExiste_DeveRetornarViewComEntregador()
    {
        // Act
        var resultado = controller.Details(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var entregador = Assert.IsInstanceOfType<EntregadorViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, entregador.Id);
        Assert.AreEqual("Carlos Silva", entregador.Nome);
        Assert.AreEqual("Motocicleta", entregador.Veiculo);
        Assert.AreEqual("ABC1234", entregador.Placa);

        pessoaServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Details_QuandoEntregadorNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Details(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pessoaServiceMock.Verify(
            service => service.Get(999),
            Times.Once);

        mapperMock.Verify(
            mapper => mapper.Map<EntregadorViewModel>(
                It.IsAny<object>()),
            Times.Never);
    }

    [TestMethod]
    public void Details_QuandoPessoaNaoEEntregador_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Details(2);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pessoaServiceMock.Verify(
            service => service.Get(2),
            Times.Once);

        mapperMock.Verify(
            mapper => mapper.Map<EntregadorViewModel>(
                It.IsAny<object>()),
            Times.Never);
    }


    [TestMethod]
    public void Create_QuandoRequisicaoGet_DeveRetornarViewComModeloInicial()
    {
        // Act
        var resultado = controller.Create();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var entregador = Assert.IsInstanceOfType<EntregadorViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(0u, entregador.Id);
        Assert.IsTrue(string.IsNullOrEmpty(entregador.Nome));
        Assert.IsTrue(string.IsNullOrEmpty(entregador.Cpf));
        Assert.IsTrue(string.IsNullOrEmpty(entregador.Email));
        Assert.IsTrue(string.IsNullOrEmpty(entregador.Veiculo));
        Assert.IsNull(entregador.DocumentoVeiculo);
    }

  

    [TestMethod]
    public async Task Create_QuandoModeloValido_DeveCriarEntregadorERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeNovoEntregador();
        ValidarModelo(modelo);

        Pessoa? entregadorCriado = null;

        pessoaServiceMock
            .Setup(service => service.Create(It.IsAny<Pessoa>()))
            .Callback<Pessoa>(entregador =>
                entregadorCriado = entregador)
            .Returns(10u);

        // Act
        var resultado = await controller.Create(modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(entregadorCriado);

        Assert.AreEqual(modelo.Nome, entregadorCriado.Nome);
        Assert.AreEqual(modelo.Cpf, entregadorCriado.Cpf);
        Assert.AreEqual(modelo.Email, entregadorCriado.Email);
        Assert.AreEqual(modelo.Telefone, entregadorCriado.Telefone);
        Assert.AreEqual(
            modelo.DataNascimento,
            entregadorCriado.DataNascimento);

        Assert.AreEqual(modelo.Veiculo, entregadorCriado.Veiculo);
        Assert.AreEqual(modelo.Placa, entregadorCriado.Placa);
        Assert.AreEqual(modelo.Cnh, entregadorCriado.Cnh);

        Assert.AreEqual("E", entregadorCriado.Papel);
        Assert.AreEqual("P", entregadorCriado.Status);

        CollectionAssert.AreEqual(
            ObterConteudoDocumento(),
            entregadorCriado.DocumentoVeiculo);

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Once);
    }

    [TestMethod]
    public async Task Create_QuandoModeloInvalido_NaoDeveCriarEntregador()
    {
        // Arrange
        var modelo = CriarModeloDeNovoEntregador();

        controller.ModelState.AddModelError(
            nameof(EntregadorViewModel.Nome),
            "O nome é obrigatório.");

        // Act
        var resultado = await controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [TestMethod]
    public async Task Create_QuandoDataNascimentoForInvalida_NaoDeveCriarEntregador()
    {
        // Arrange
        var modelo = CriarModeloDeNovoEntregador();
        modelo.DataNascimento = default;

        ValidarModelo(modelo);

        // Act
        var resultado = await controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(
            nameof(EntregadorViewModel.DataNascimento)));

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [TestMethod]
    public async Task Create_QuandoDataNascimentoForFutura_NaoDeveCriarEntregador()
    {
        // Arrange
        var modelo = CriarModeloDeNovoEntregador();
        modelo.DataNascimento = DateTime.Today.AddDays(1);

        ValidarModelo(modelo);

        // Act
        var resultado = await controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(
            nameof(EntregadorViewModel.DataNascimento)));

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [TestMethod]
    public async Task Create_QuandoDocumentoVeiculoNaoForEnviado_NaoDeveCriarEntregador()
    {
        // Arrange
        var modelo = CriarModeloDeNovoEntregador();
        modelo.DocumentoVeiculo = null;

        ValidarModelo(modelo);

        // Act
        var resultado = await controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(
            nameof(EntregadorViewModel.DocumentoVeiculo)));

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [TestMethod]
    public async Task Create_QuandoDocumentoVeiculoEstiverVazio_NaoDeveCriarEntregador()
    {
        // Arrange
        var modelo = CriarModeloDeNovoEntregador();
        modelo.DocumentoVeiculo = CriarArquivoVazio();

        ValidarModelo(modelo);

        // Act
        var resultado = await controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(
            nameof(EntregadorViewModel.DocumentoVeiculo)));

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [TestMethod]
    public async Task Create_QuandoOcorrerErroNoBanco_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDeNovoEntregador();
        ValidarModelo(modelo);

        pessoaServiceMock
            .Setup(service => service.Create(It.IsAny<Pessoa>()))
            .Throws(new DbUpdateException("Erro ao salvar"));

        // Act
        var resultado = await controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Once);
    }

  
    [TestMethod]
    public void Edit_QuandoEntregadorExiste_DeveRetornarViewComEntregador()
    {
        // Act
        var resultado = controller.Edit(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var entregador = Assert.IsInstanceOfType<EntregadorViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, entregador.Id);
        Assert.AreEqual("Carlos Silva", entregador.Nome);
        Assert.AreEqual("Motocicleta", entregador.Veiculo);

        pessoaServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoEntregadorNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Edit(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pessoaServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoPessoaNaoEEntregador_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Edit(2);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pessoaServiceMock.Verify(
            service => service.Get(2),
            Times.Once);
    }

  

    [TestMethod]
    public async Task Edit_QuandoModeloValido_DeveAtualizarEntregadorERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeEntregadorExistente();
        ValidarModelo(modelo);

        Pessoa? entregadorEditado = null;

        pessoaServiceMock
            .Setup(service => service.Edit(It.IsAny<Pessoa>()))
            .Callback<Pessoa>(entregador =>
                entregadorEditado = entregador);

        // Act
        var resultado = await controller.Edit(modelo.Id, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(entregadorEditado);

        Assert.AreEqual(modelo.Id, entregadorEditado.Id);
        Assert.AreEqual(modelo.Nome, entregadorEditado.Nome);
        Assert.AreEqual(modelo.Cpf, entregadorEditado.Cpf);
        Assert.AreEqual(modelo.Email, entregadorEditado.Email);
        Assert.AreEqual(modelo.Telefone, entregadorEditado.Telefone);
        Assert.AreEqual(
            modelo.DataNascimento,
            entregadorEditado.DataNascimento);

        Assert.AreEqual(modelo.Veiculo, entregadorEditado.Veiculo);
        Assert.AreEqual(modelo.Placa, entregadorEditado.Placa);
        Assert.AreEqual(modelo.Cnh, entregadorEditado.Cnh);

        pessoaServiceMock.Verify(
            service => service.Get(modelo.Id),
            Times.Once);

        pessoaServiceMock.Verify(
            service => service.Edit(It.IsAny<Pessoa>()),
            Times.Once);
    }

    [TestMethod]
    public async Task Edit_QuandoIdsForemDiferentes_DeveRetornarBadRequest()
    {
        // Arrange
        var modelo = CriarModeloDeEntregadorExistente();

        // Act
        var resultado = await controller.Edit(999, modelo);

        // Assert
        Assert.IsInstanceOfType<BadRequestResult>(resultado);

        pessoaServiceMock.Verify(
            service => service.Get(It.IsAny<uint>()),
            Times.Never);

        pessoaServiceMock.Verify(
            service => service.Edit(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [TestMethod]
    public async Task Edit_QuandoModeloInvalido_NaoDeveAtualizarEntregador()
    {
        // Arrange
        var modelo = CriarModeloDeEntregadorExistente();

        controller.ModelState.AddModelError(
            nameof(EntregadorViewModel.Nome),
            "O nome é obrigatório.");

        // Act
        var resultado = await controller.Edit(modelo.Id, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        pessoaServiceMock.Verify(
            service => service.Get(It.IsAny<uint>()),
            Times.Never);

        pessoaServiceMock.Verify(
            service => service.Edit(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [TestMethod]
    public async Task Edit_Post_QuandoEntregadorNaoExiste_DeveRetornarNotFound()
    {
        // Arrange
        var modelo = CriarModeloDeEntregadorExistente();
        modelo.Id = 999;

        ValidarModelo(modelo);

        // Act
        var resultado = await controller.Edit(999, modelo);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pessoaServiceMock.Verify(
            service => service.Get(999),
            Times.Once);

        pessoaServiceMock.Verify(
            service => service.Edit(It.IsAny<Pessoa>()),
            Times.Never);
    }

    
  

    [TestMethod]
    public async Task Edit_QuandoDocumentoNaoForEnviado_DeveManterDocumentoExistente()
    {
        // Arrange
        var entregadorOriginal = ObterEntregadorAlvo();
        entregadorOriginal.DocumentoVeiculo = [1, 2, 3, 4];

        pessoaServiceMock
            .Setup(service => service.Get(1))
            .Returns(entregadorOriginal);

        var modelo = CriarModeloDeEntregadorExistente();
        modelo.DocumentoVeiculo = null;

        ValidarModelo(modelo);

        Pessoa? entregadorEditado = null;

        pessoaServiceMock
            .Setup(service => service.Edit(It.IsAny<Pessoa>()))
            .Callback<Pessoa>(entregador =>
                entregadorEditado = entregador);

        // Act
        var resultado = await controller.Edit(1, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(entregadorEditado);

        CollectionAssert.AreEqual(
            new byte[] { 1, 2, 3, 4 },
            entregadorEditado.DocumentoVeiculo);

        pessoaServiceMock.Verify(
            service => service.Edit(It.IsAny<Pessoa>()),
            Times.Once);
    }

    [TestMethod]
    public async Task Edit_QuandoNovoDocumentoForEnviado_DeveSubstituirDocumentoExistente()
    {
        // Arrange
        var entregadorOriginal = ObterEntregadorAlvo();
        entregadorOriginal.DocumentoVeiculo = [1, 2, 3];

        pessoaServiceMock
            .Setup(service => service.Get(1))
            .Returns(entregadorOriginal);

        var modelo = CriarModeloDeEntregadorExistente();
        modelo.DocumentoVeiculo = CriarArquivoDeTeste();

        ValidarModelo(modelo);

        Pessoa? entregadorEditado = null;

        pessoaServiceMock
            .Setup(service => service.Edit(It.IsAny<Pessoa>()))
            .Callback<Pessoa>(entregador =>
                entregadorEditado = entregador);

        // Act
        var resultado = await controller.Edit(1, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(entregadorEditado);

        CollectionAssert.AreEqual(
            ObterConteudoDocumento(),
            entregadorEditado.DocumentoVeiculo);

        pessoaServiceMock.Verify(
            service => service.Edit(It.IsAny<Pessoa>()),
            Times.Once);
    }

    // =========================================================
    // DELETE - GET
    // =========================================================

    [TestMethod]
    public void Delete_QuandoEntregadorExiste_DeveRetornarViewComEntregador()
    {
        // Act
        var resultado = controller.Delete(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var entregador = Assert.IsInstanceOfType<EntregadorViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, entregador.Id);
        Assert.AreEqual("Carlos Silva", entregador.Nome);

        pessoaServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoEntregadorNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Delete(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pessoaServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoPessoaNaoEEntregador_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Delete(2);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pessoaServiceMock.Verify(
            service => service.Get(2),
            Times.Once);
    }

    // =========================================================
    // DELETE - POST
    // =========================================================

    [TestMethod]
    public void Delete_QuandoEntregadorExiste_DeveDesativarERedirecionarParaIndex()
    {
        // Arrange
        var collectionMock = new Mock<IFormCollection>();

        // Act
        var resultado = controller.Delete(1, collectionMock.Object);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        pessoaServiceMock.Verify(
            service => service.Get(1),
            Times.Once);

        pessoaServiceMock.Verify(
            service => service.Delete(1),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoEntregadorNaoExiste_NaoDeveDesativar()
    {
        // Arrange
        var collectionMock = new Mock<IFormCollection>();

        // Act
        var resultado = controller.Delete(999, collectionMock.Object);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pessoaServiceMock.Verify(
            service => service.Delete(It.IsAny<uint>()),
            Times.Never);
    }

    [TestMethod]
    public void Delete_QuandoPessoaNaoEEntregador_NaoDeveDesativar()
    {
        // Arrange
        var collectionMock = new Mock<IFormCollection>();

        // Act
        var resultado = controller.Delete(2, collectionMock.Object);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pessoaServiceMock.Verify(
            service => service.Delete(It.IsAny<uint>()),
            Times.Never);
    }


    private static void VerificarRedirecionamentoParaIndex(
        ActionResult resultado)
    {
        var redirectResult =
            Assert.IsInstanceOfType<RedirectToActionResult>(resultado);

        Assert.IsNull(redirectResult.ControllerName);

        Assert.AreEqual(
            nameof(EntregadorController.Index),
            redirectResult.ActionName);
    }

    private static EntregadorViewModel CriarModeloDeNovoEntregador()
    {
        return new EntregadorViewModel
        {
            Nome = "Maria Oliveira",
            Cpf = "12345678901",
            Email = "maria@email.com",
            Telefone = "79999999999",
            DataNascimento = new DateTime(1995, 5, 10),
            Veiculo = "Motocicleta",
            Placa = "ABC1234",
            Cnh = "12345678901",
            DocumentoVeiculo = CriarArquivoDeTeste(),
            Status = "A"
        };
    }

    private static EntregadorViewModel CriarModeloDeEntregadorExistente()
    {
        return new EntregadorViewModel
        {
            Id = 1,
            Nome = "Carlos Silva Atualizado",
            Cpf = "12345678901",
            Email = "carlos.atualizado@email.com",
            Telefone = "79999999999",
            DataNascimento = new DateTime(1995, 5, 10),
            Veiculo = "Carro",
            Placa = "XYZ1234",
            Cnh = "12345678901",
            DocumentoVeiculo = null,
            Status = "A"
        };
    }

    private static Pessoa ObterEntregadorAlvo()
    {
        return new Pessoa
        {
            Id = 1,
            Nome = "Carlos Silva",
            Cpf = "12345678901",
            Email = "carlos@email.com",
            Telefone = "79999999999",
            DataNascimento = new DateTime(1995, 5, 10),
            Veiculo = "Motocicleta",
            Placa = "ABC1234",
            Cnh = "12345678901",
            DocumentoVeiculo = [1, 2, 3, 4],
            Papel = "E",
            Status = "A"
        };
    }

    private static Pessoa ObterSolicitante()
    {
        return new Pessoa
        {
            Id = 2,
            Nome = "João Santos",
            Cpf = "98765432100",
            Email = "joao@email.com",
            Telefone = "79988887777",
            DataNascimento = new DateTime(1990, 3, 15),
            Papel = "S",
            Status = "A"
        };
    }

    private static IEnumerable<Pessoa> ObterPessoasDeTeste()
    {
        return
        [
            ObterEntregadorAlvo(),
            ObterSolicitante(),
            new Pessoa
            {
                Id = 3,
                Nome = "Pedro Oliveira",
                Cpf = "11122233344",
                Email = "pedro@email.com",
                Telefone = "79977776666",
                DataNascimento = new DateTime(1998, 8, 20),
                Veiculo = "Carro",
                Placa = "DEF5678",
                Cnh = "98765432100",
                Papel = "E",
                Status = "A"
            }
        ];
    }

    private static EntregadorViewModel MapearEntregador(Pessoa pessoa)
    {
        return new EntregadorViewModel
        {
            Id = pessoa.Id,
            Nome = pessoa.Nome,
            Cpf = pessoa.Cpf,
            Email = pessoa.Email,
            Telefone = pessoa.Telefone,
            DataNascimento = pessoa.DataNascimento,
            Veiculo = pessoa.Veiculo,
            Placa = pessoa.Placa,
            Cnh = pessoa.Cnh,
            Status = pessoa.Status
        };
    }

    private static IFormFile CriarArquivoDeTeste()
    {
        var arquivoMock = new Mock<IFormFile>();

        var conteudo = ObterConteudoDocumento();

        arquivoMock
            .Setup(arquivo => arquivo.Length)
            .Returns(conteudo.Length);

        arquivoMock
            .Setup(arquivo => arquivo.CopyToAsync(
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()))
            .Returns((Stream destino, CancellationToken _) =>
            {
                destino.Write(conteudo, 0, conteudo.Length);
                return Task.CompletedTask;
            });

        return arquivoMock.Object;
    }

    private static IFormFile CriarArquivoVazio()
    {
        var arquivoMock = new Mock<IFormFile>();

        arquivoMock
            .Setup(arquivo => arquivo.Length)
            .Returns(0);

        return arquivoMock.Object;
    }

    private static byte[] ObterConteudoDocumento()
    {
        return [10, 20, 30, 40, 50];
    }

    private void ValidarModelo(EntregadorViewModel modelo)
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
