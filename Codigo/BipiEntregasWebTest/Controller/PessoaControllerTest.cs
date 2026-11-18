
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
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
public class PessoaControllerTests
{
    private Mock<IPessoaService> pessoaServiceMock = null!;
    private Mock<IMapper> mapperMock = null!;
    private PessoaController controller = null!;
    private SessaoDeTeste sessao = null!;

    [TestInitialize]
    public void Inicializar()
    {
        pessoaServiceMock = new Mock<IPessoaService>();
        mapperMock = new Mock<IMapper>();

        pessoaServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterPessoasDeTeste());

        pessoaServiceMock
            .Setup(service => service.GetAll("A"))
            .Returns(ObterPessoasDeTeste()
                .Where(pessoa => pessoa.Status == "A")
                .ToList());

        pessoaServiceMock
            .Setup(service => service.Get(1))
            .Returns(ObterPessoaAlvo());

        pessoaServiceMock
            .Setup(service => service.Get(999))
            .Returns((Pessoa?)null);

        mapperMock
            .Setup(mapper => mapper.Map<List<PessoaViewModel>>(
                It.IsAny<object>()))
            .Returns((object origem) =>
            {
                var pessoas = (IEnumerable<Pessoa>)origem;

                return pessoas.Select(MapearPessoa).ToList();
            });

        mapperMock
            .Setup(mapper => mapper.Map<PessoaViewModel>(
                It.IsAny<object>()))
            .Returns((object origem) =>
                MapearPessoa((Pessoa)origem));

        controller = new PessoaController(
            pessoaServiceMock.Object,
            mapperMock.Object);

        // Sessão em memória, usada pelo Login (perfil ativo - UC07).
        sessao = new SessaoDeTeste();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Session = sessao
            }
        };
    }

    // =========================================================
    // INDEX
    // =========================================================

    [TestMethod]
    public void Index_QuandoChamado_DeveRetornarViewComTodasAsPessoas()
    {
        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var pessoas = Assert.IsInstanceOfType<List<PessoaViewModel>>(
            viewResult.ViewData.Model);

        Assert.AreEqual(4, pessoas.Count);

        Assert.IsTrue(pessoas.All(pessoa =>
            !string.IsNullOrEmpty(pessoa.Nome)));

        CollectionAssert.AreEquivalent(
            new[] { "S", "E", "A", "S" },
            pessoas.Select(pessoa => pessoa.Papel).ToList());

        pessoaServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    [TestMethod]
    public void Index_QuandoNaoExistemPessoas_DeveRetornarViewComColecaoVazia()
    {
        // Arrange
        pessoaServiceMock
            .Setup(service => service.GetAll())
            .Returns(Array.Empty<Pessoa>());

        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var pessoas = Assert.IsInstanceOfType<List<PessoaViewModel>>(
            viewResult.ViewData.Model);

        Assert.IsEmpty(pessoas);

        pessoaServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    // =========================================================
    // DETAILS
    // =========================================================

    [TestMethod]
    public void Details_QuandoPessoaExiste_DeveRetornarViewComPessoa()
    {
        // Act
        var resultado = controller.Details(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var pessoa = Assert.IsInstanceOfType<PessoaViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, pessoa.Id);
        Assert.AreEqual("Ana Souza", pessoa.Nome);
        Assert.AreEqual("S", pessoa.Papel);
        Assert.AreEqual("A", pessoa.Status);

        pessoaServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Details_QuandoPessoaNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Details(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pessoaServiceMock.Verify(
            service => service.Get(999),
            Times.Once);

        mapperMock.Verify(
            mapper => mapper.Map<PessoaViewModel>(
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

        var pessoa = Assert.IsInstanceOfType<PessoaViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(0u, pessoa.Id);
        Assert.IsTrue(string.IsNullOrEmpty(pessoa.Nome));
        Assert.IsTrue(string.IsNullOrEmpty(pessoa.Cpf));
        Assert.IsTrue(string.IsNullOrEmpty(pessoa.Email));
        Assert.IsTrue(string.IsNullOrEmpty(pessoa.Papel));
    }

    // =========================================================
    // CREATE - POST
    // =========================================================

    [TestMethod]
    public void Create_QuandoSolicitanteValido_DeveCriarAtivoERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeNovaPessoa("S");
        ValidarModelo(modelo);

        Pessoa? pessoaCriada = null;

        pessoaServiceMock
            .Setup(service => service.Create(It.IsAny<Pessoa>()))
            .Callback<Pessoa>(pessoa =>
                pessoaCriada = pessoa)
            .Returns(10u);

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(pessoaCriada);

        Assert.AreEqual(modelo.Nome, pessoaCriada.Nome);
        Assert.AreEqual(modelo.Cpf, pessoaCriada.Cpf);
        Assert.AreEqual(modelo.Email, pessoaCriada.Email);
        Assert.AreEqual(modelo.Telefone, pessoaCriada.Telefone);
        Assert.AreEqual(
            modelo.DataNascimento,
            pessoaCriada.DataNascimento);

        Assert.AreEqual("S", pessoaCriada.Papel);
        Assert.AreEqual("A", pessoaCriada.Status);

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Once);
    }

    [TestMethod]
    public void Create_QuandoEntregadorValido_DeveCriarComStatusPendente()
    {
        // Arrange
        var modelo = CriarModeloDeNovaPessoa("E");
        ValidarModelo(modelo);

        Pessoa? pessoaCriada = null;

        pessoaServiceMock
            .Setup(service => service.Create(It.IsAny<Pessoa>()))
            .Callback<Pessoa>(pessoa =>
                pessoaCriada = pessoa)
            .Returns(11u);

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(pessoaCriada);

        // Entregador aguarda validação do cadastro (CSU10).
        Assert.AreEqual("E", pessoaCriada.Papel);
        Assert.AreEqual("P", pessoaCriada.Status);
    }

    [TestMethod]
    public void Create_QuandoModeloInvalido_NaoDeveCriarPessoa()
    {
        // Arrange
        var modelo = CriarModeloDeNovaPessoa("S");

        controller.ModelState.AddModelError(
            nameof(PessoaViewModel.Nome),
            "Informe o nome.");

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [TestMethod]
    public void Create_QuandoDataNascimentoForInvalida_NaoDeveCriarPessoa()
    {
        // Arrange
        var modelo = CriarModeloDeNovaPessoa("S");
        modelo.DataNascimento = default;

        ValidarModelo(modelo);

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(
            nameof(PessoaViewModel.DataNascimento)));

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [TestMethod]
    public void Create_QuandoDataNascimentoForFutura_NaoDeveCriarPessoa()
    {
        // Arrange
        var modelo = CriarModeloDeNovaPessoa("S");
        modelo.DataNascimento = DateTime.Today.AddDays(1);

        ValidarModelo(modelo);

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(
            nameof(PessoaViewModel.DataNascimento)));

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [TestMethod]
    public void Create_QuandoPerfilForInvalido_NaoDeveCriarPessoa()
    {
        // Arrange
        var modelo = CriarModeloDeNovaPessoa("X");
        ValidarModelo(modelo);

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(
            nameof(PessoaViewModel.Papel)));

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [TestMethod]
    public void Create_QuandoOcorrerErroNoBanco_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDeNovaPessoa("S");
        ValidarModelo(modelo);

        pessoaServiceMock
            .Setup(service => service.Create(It.IsAny<Pessoa>()))
            .Throws(new DbUpdateException("Erro ao salvar"));

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Once);
    }

    // =========================================================
    // EDIT - GET
    // =========================================================

    [TestMethod]
    public void Edit_QuandoPessoaExiste_DeveRetornarViewComPessoa()
    {
        // Act
        var resultado = controller.Edit(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var pessoa = Assert.IsInstanceOfType<PessoaViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, pessoa.Id);
        Assert.AreEqual("Ana Souza", pessoa.Nome);

        pessoaServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoPessoaNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Edit(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pessoaServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    // =========================================================
    // EDIT - POST
    // =========================================================

    [TestMethod]
    public void Edit_QuandoModeloValido_DeveAtualizarPessoaERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDePessoaExistente();
        ValidarModelo(modelo);

        Pessoa? pessoaEditada = null;

        pessoaServiceMock
            .Setup(service => service.Edit(It.IsAny<Pessoa>()))
            .Callback<Pessoa>(pessoa =>
                pessoaEditada = pessoa);

        // Act
        var resultado = controller.Edit(modelo.Id, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(pessoaEditada);

        Assert.AreEqual(modelo.Id, pessoaEditada.Id);
        Assert.AreEqual(modelo.Nome, pessoaEditada.Nome);
        Assert.AreEqual(modelo.Email, pessoaEditada.Email);
        Assert.AreEqual(modelo.Telefone, pessoaEditada.Telefone);
        Assert.AreEqual(
            modelo.DataNascimento,
            pessoaEditada.DataNascimento);

        // CPF e perfil não são alterados na edição.
        Assert.AreEqual("12345678901", pessoaEditada.Cpf);
        Assert.AreEqual("S", pessoaEditada.Papel);

        pessoaServiceMock.Verify(
            service => service.Get(modelo.Id),
            Times.Once);

        pessoaServiceMock.Verify(
            service => service.Edit(It.IsAny<Pessoa>()),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoIdsForemDiferentes_DeveRetornarBadRequest()
    {
        // Arrange
        var modelo = CriarModeloDePessoaExistente();

        // Act
        var resultado = controller.Edit(999, modelo);

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
    public void Edit_QuandoModeloInvalido_NaoDeveAtualizarPessoa()
    {
        // Arrange
        var modelo = CriarModeloDePessoaExistente();

        controller.ModelState.AddModelError(
            nameof(PessoaViewModel.Nome),
            "Informe o nome.");

        // Act
        var resultado = controller.Edit(modelo.Id, modelo);

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
    public void Edit_Post_QuandoPessoaNaoExiste_DeveRetornarNotFound()
    {
        // Arrange
        var modelo = CriarModeloDePessoaExistente();
        modelo.Id = 999;

        ValidarModelo(modelo);

        // Act
        var resultado = controller.Edit(999, modelo);

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
    public void Edit_QuandoOcorrerErroNoBanco_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDePessoaExistente();
        ValidarModelo(modelo);

        pessoaServiceMock
            .Setup(service => service.Edit(It.IsAny<Pessoa>()))
            .Throws(new DbUpdateException("Erro ao salvar"));

        // Act
        var resultado = controller.Edit(modelo.Id, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));
    }

    // =========================================================
    // DELETE - GET
    // =========================================================

    [TestMethod]
    public void Delete_QuandoPessoaExiste_DeveRetornarViewComPessoa()
    {
        // Act
        var resultado = controller.Delete(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        var pessoa = Assert.IsInstanceOfType<PessoaViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, pessoa.Id);
        Assert.AreEqual("Ana Souza", pessoa.Nome);

        pessoaServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoPessoaNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Delete(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        pessoaServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    // =========================================================
    // DELETE - POST
    // =========================================================

    [TestMethod]
    public void Delete_QuandoPessoaExiste_DeveDesativarERedirecionarParaIndex()
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
    public void Delete_QuandoPessoaNaoExiste_NaoDeveDesativar()
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

    // =========================================================
    // LOGIN - GET
    // =========================================================

    [TestMethod]
    public void Login_QuandoRequisicaoGet_DeveRetornarView()
    {
        // Act
        var resultado = controller.Login();

        // Assert
        Assert.IsInstanceOfType<ViewResult>(resultado);
    }

    // =========================================================
    // LOGIN - POST (perfil ativo na sessão - UC07)
    // =========================================================

    [TestMethod]
    public void Login_QuandoDadosValidos_DeveGuardarPerfilNaSessaoERedirecionarParaHome()
    {
        // Act
        var resultado = controller.Login("ana@email.com", "12345678901");

        // Assert
        var redirectResult =
            Assert.IsInstanceOfType<RedirectToActionResult>(resultado);

        Assert.AreEqual("Home", redirectResult.ControllerName);
        Assert.AreEqual("Index", redirectResult.ActionName);

        Assert.AreEqual(1, sessao.GetInt32("IdPessoa"));
        Assert.AreEqual("Ana Souza", sessao.GetString("NomePessoa"));
        Assert.AreEqual("S", sessao.GetString("PerfilAtivo"));

        pessoaServiceMock.Verify(
            service => service.GetAll("A"),
            Times.Once);
    }

    [TestMethod]
    public void Login_QuandoUsuarioForEntregador_DeveDefinirPerfilAtivoComoEntregador()
    {
        // Act
        var resultado = controller.Login("carlos@email.com", "98765432100");

        // Assert
        Assert.IsInstanceOfType<RedirectToActionResult>(resultado);

        Assert.AreEqual(2, sessao.GetInt32("IdPessoa"));
        Assert.AreEqual("E", sessao.GetString("PerfilAtivo"));
    }

    [TestMethod]
    public void Login_QuandoEmailOuCpfEmBranco_NaoDeveConsultarPessoas()
    {
        // Act
        var resultado = controller.Login("", "");

        // Assert
        Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));

        Assert.IsNull(sessao.GetString("PerfilAtivo"));

        pessoaServiceMock.Verify(
            service => service.GetAll(It.IsAny<string>()),
            Times.Never);
    }

    [TestMethod]
    public void Login_QuandoDadosNaoConferem_DeveRetornarViewComMensagemDeErro()
    {
        // Act
        var resultado = controller.Login("ana@email.com", "00000000000");

        // Assert
        Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));

        Assert.IsNull(sessao.GetString("PerfilAtivo"));
    }

    [TestMethod]
    public void Login_QuandoCadastroInativo_NaoDeveAutenticar()
    {
        // Act
        // Pessoa 4 existe, mas está Inativa (Status = "I").
        var resultado = controller.Login("rita@email.com", "55566677788");

        // Assert
        Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));

        Assert.IsNull(sessao.GetString("PerfilAtivo"));

        pessoaServiceMock.Verify(
            service => service.GetAll("A"),
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
            nameof(PessoaController.Index),
            redirectResult.ActionName);
    }

    private static PessoaViewModel CriarModeloDeNovaPessoa(string papel)
    {
        return new PessoaViewModel
        {
            Nome = "Maria Oliveira",
            Cpf = "11122233344",
            Email = "maria@email.com",
            Telefone = "79999999999",
            DataNascimento = new DateTime(1995, 5, 10),
            Papel = papel,
            Status = "A"
        };
    }

    private static PessoaViewModel CriarModeloDePessoaExistente()
    {
        return new PessoaViewModel
        {
            Id = 1,
            Nome = "Ana Souza Atualizada",
            Cpf = "12345678901",
            Email = "ana.atualizada@email.com",
            Telefone = "79911112222",
            DataNascimento = new DateTime(1996, 6, 20),
            Papel = "S",
            Status = "A"
        };
    }

    private static Pessoa ObterPessoaAlvo()
    {
        return new Pessoa
        {
            Id = 1,
            Nome = "Ana Souza",
            Cpf = "12345678901",
            Email = "ana@email.com",
            Telefone = "79999999999",
            DataNascimento = new DateTime(1996, 6, 20),
            Papel = "S",
            Status = "A"
        };
    }

    private static IEnumerable<Pessoa> ObterPessoasDeTeste()
    {
        return
        [
            ObterPessoaAlvo(),
            new Pessoa
            {
                Id = 2,
                Nome = "Carlos Silva",
                Cpf = "98765432100",
                Email = "carlos@email.com",
                Telefone = "79988887777",
                DataNascimento = new DateTime(1990, 3, 15),
                Veiculo = "Motocicleta",
                Placa = "ABC1234",
                Cnh = "12345678901",
                Papel = "E",
                Status = "A"
            },
            new Pessoa
            {
                Id = 3,
                Nome = "Admin BiPi",
                Cpf = "44455566677",
                Email = "admin@email.com",
                Telefone = "79977776666",
                DataNascimento = new DateTime(1988, 1, 1),
                Papel = "A",
                Status = "A"
            },
            new Pessoa
            {
                Id = 4,
                Nome = "Rita Lima",
                Cpf = "55566677788",
                Email = "rita@email.com",
                Telefone = "79966665555",
                DataNascimento = new DateTime(1999, 9, 9),
                Papel = "S",
                Status = "I"
            }
        ];
    }

    private static PessoaViewModel MapearPessoa(Pessoa pessoa)
    {
        return new PessoaViewModel
        {
            Id = pessoa.Id,
            Nome = pessoa.Nome,
            Cpf = pessoa.Cpf,
            Email = pessoa.Email,
            Telefone = pessoa.Telefone,
            DataNascimento = pessoa.DataNascimento,
            Papel = pessoa.Papel,
            Status = pessoa.Status
        };
    }

    private void ValidarModelo(PessoaViewModel modelo)
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

    // Sessão em memória para os testes do Login.
    private class SessaoDeTeste : ISession
    {
        private readonly Dictionary<string, byte[]> _valores = new();

        public bool IsAvailable => true;

        public string Id => "sessao-de-teste";

        public IEnumerable<string> Keys => _valores.Keys;

        public void Clear() => _valores.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public void Remove(string key) => _valores.Remove(key);

        public void Set(string key, byte[] value) => _valores[key] = value;

        public bool TryGetValue(string key, [NotNullWhen(true)] out byte[]? value)
            => _valores.TryGetValue(key, out value);
    }
}