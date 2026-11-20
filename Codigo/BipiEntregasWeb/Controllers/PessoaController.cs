using AutoMapper;
using BipiEntregasWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BipiEntregasWeb.Controllers
{
    public class PessoaController : Controller
    {

        // Controller utiliza os métodos do PessoaService para consultar,
        // cadastrar, editar e desativar pessoas de qualquer perfil
        // (S = Solicitante, E = Entregador, A = Administrador)
        // e para realizar o login do usuário.

        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;

        public PessoaController(
            IPessoaService pessoaService,
            IMapper mapper)
        {
            _pessoaService = pessoaService;
            _mapper = mapper;
        }



        // GET: PessoaController
        public ActionResult Index()
        {
            var pessoas = _pessoaService.GetAll()
                .ToList();

            var pessoasViewModel =
                _mapper.Map<List<PessoaViewModel>>(pessoas);

            return View(pessoasViewModel);
        }

        // GET: PessoaController/Details/5
        public ActionResult Details(uint id)
        {
            var pessoa = _pessoaService.Get(id);

            if (pessoa == null)
            {
                return NotFound();
            }

            var pessoaViewModel =
                _mapper.Map<PessoaViewModel>(pessoa);

            return View(pessoaViewModel);
        }

        // GET: PessoaController/Create
        public ActionResult Create()
        {
            var pessoaViewModel = new PessoaViewModel();
            return View(pessoaViewModel);
        }

        // POST: PessoaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PessoaViewModel pessoaViewModel)
        {
            if (pessoaViewModel.DataNascimento == default ||
                pessoaViewModel.DataNascimento.Date > DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(pessoaViewModel.DataNascimento),
                    "Informe uma data de nascimento válida.");
            }

            // O Status é definido automaticamente pelo sistema.
            ModelState.Remove(nameof(PessoaViewModel.Status));

            if (!ModelState.IsValid)
            {
                return View(pessoaViewModel);
            }

            var pessoa = new Pessoa
            {
                Nome = pessoaViewModel.Nome,
                Cpf = pessoaViewModel.Cpf,
                Email = pessoaViewModel.Email,
                Telefone = pessoaViewModel.Telefone,
                DataNascimento = pessoaViewModel.DataNascimento,
                Papel = pessoaViewModel.Papel,

                // Entregador começa Pendente (validação do cadastro - CSU10);
                // os demais perfis começam Ativos.
                Status = pessoaViewModel.Papel == "E" ? "P" : "A"
            };

            try
            {
                _pessoaService.Create(pessoa);
                TempData["Mensagem"] = "Cadastro realizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível cadastrar a pessoa. " +
                    "Verifique os dados e se CPF ou e-mail já estão cadastrados.");

                return View(pessoaViewModel);
            }
        }

        // GET: PessoaController/Edit/5
        public ActionResult Edit(uint id)
        {
            var pessoa = _pessoaService.Get(id);

            if (pessoa == null)
            {
                return NotFound();
            }

            var pessoaViewModel =
                _mapper.Map<PessoaViewModel>(pessoa);

            return View(pessoaViewModel);
        }


        // POST: PessoaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(uint id,
            PessoaViewModel pessoaViewModel)
        {
            if (id != pessoaViewModel.Id)
            {
                return BadRequest();
            }

            // O Status não é editado pelo formulário.
            ModelState.Remove(nameof(PessoaViewModel.Status));

            if (!ModelState.IsValid)
            {
                return View(pessoaViewModel);
            }

            var pessoa = _pessoaService.Get(id);

            if (pessoa == null)
            {
                return NotFound();
            }

            // CPF e perfil não são alterados na edição.
            pessoa.Nome = pessoaViewModel.Nome;
            pessoa.Email = pessoaViewModel.Email;
            pessoa.Telefone = pessoaViewModel.Telefone;
            pessoa.DataNascimento = pessoaViewModel.DataNascimento;

            try
            {
                _pessoaService.Edit(pessoa);
                TempData["Mensagem"] = "Dados alterados com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível alterar a pessoa. " +
                    "Verifique se o e-mail já está cadastrado.");

                return View(pessoaViewModel);
            }
        }

        // GET: PessoaController/Delete/5
        public ActionResult Delete(uint id)
        {
            var pessoa = _pessoaService.Get(id);

            if (pessoa == null)
            {
                return NotFound();
            }

            var pessoaViewModel =
                _mapper.Map<PessoaViewModel>(pessoa);

            return View(pessoaViewModel);
        }

        // POST: PessoaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, IFormCollection collection)
        {
            var pessoa = _pessoaService.Get(id);

            if (pessoa == null)
            {
                return NotFound();
            }

            // Desativação lógica: altera Status para "I".
            _pessoaService.Delete(id);

            TempData["Mensagem"] = "Cadastro desativado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // GET: PessoaController/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: PessoaController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string cpf)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(cpf))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Informe o e-mail e o CPF.");

                return View();
            }

            // Somente cadastros ativos podem acessar o sistema.
            // TODO: trocar o CPF pela senha quando existir no banco (CSU02).
            var pessoa = _pessoaService.GetAll("A")
                .FirstOrDefault(p =>
                    p.Email == email &&
                    p.Cpf == cpf);

            if (pessoa == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "E-mail ou CPF inválidos, ou cadastro não está ativo.");

                return View();
            }

            // Guarda o usuário logado e o perfil ativo na sessão.
            // O perfil ativo é usado pelo caso de uso Alternar Perfil (UC07).
            HttpContext.Session.SetInt32("IdPessoa", (int)pessoa.Id);
            HttpContext.Session.SetString("NomePessoa", pessoa.Nome);
            HttpContext.Session.SetString("PerfilAtivo", pessoa.Papel);

            return RedirectToAction("Index", "Home");
        }
    }
}