
using AutoMapper;
using BipiEntregasWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Core;
using Microsoft.EntityFrameworkCore;

namespace BipiEntregasWeb.Controllers
{
    public class SolicitanteController : Controller
    {
        // Controller utiliza os métodos do PessoaService para consultar,
        // cadastrar, editar e desativar pessoas do tipo Solicitante (Papel = S).
        // O Solicitante é um usuário que solicita entregas.

        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;

        public SolicitanteController(
            IPessoaService pessoaService,
            IMapper mapper)
        {
            _pessoaService = pessoaService;
            _mapper = mapper;
        }

        // GET: SolicitanteController
        public ActionResult Index()
        {
            var solicitantes = _pessoaService.GetAll()
                .Where(p => p.Papel == "S")
                .ToList();

            var solicitantesViewModel =
                _mapper.Map<List<SolicitanteViewModel>>(solicitantes);

            return View(solicitantesViewModel);
        }

        // GET: SolicitanteController/Details/5
        public ActionResult Details(uint id)
        {
            var pessoa = _pessoaService.Get(id);

            if (pessoa == null || pessoa.Papel != "S")
            {
                return NotFound();
            }

            var solicitanteViewModel =
                _mapper.Map<SolicitanteViewModel>(pessoa);

            return View(solicitanteViewModel);
        }

        // GET: SolicitanteController/Create
        public ActionResult Create()
        {
            var solicitanteViewModel = new SolicitanteViewModel();

            return View(solicitanteViewModel);
        }

        // POST: SolicitanteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SolicitanteViewModel solicitanteViewModel)
        {
            if (solicitanteViewModel.DataNascimento == default ||
                solicitanteViewModel.DataNascimento.Date > DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(solicitanteViewModel.DataNascimento),
                    "Informe uma data de nascimento válida.");
            }

            // O Status é definido automaticamente pelo sistema.
            // Não é necessário recebê-lo do formulário.
            ModelState.Remove(nameof(SolicitanteViewModel.Status));

            if (!ModelState.IsValid)
            {
                return View(solicitanteViewModel);
            }

            var pessoa = new Pessoa
            {
                Nome = solicitanteViewModel.Nome,
                Cpf = solicitanteViewModel.Cpf,
                Email = solicitanteViewModel.Email,
                Telefone = solicitanteViewModel.Telefone,
                DataNascimento = solicitanteViewModel.DataNascimento,
                Cep = solicitanteViewModel.Cep,
                Cidade = solicitanteViewModel.Cidade,
                Rua = solicitanteViewModel.Rua,
                Numero = solicitanteViewModel.Numero,
                Complemento = solicitanteViewModel.Complemento,

                Papel = "S",
                Status = "A"
            };

            try
            {
                _pessoaService.Create(pessoa);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível cadastrar o solicitante. " +
                    "Verifique os dados e se CPF ou e-mail já estão cadastrados.");

                return View(solicitanteViewModel);
            }
        }

        // GET: SolicitanteController/Edit/5
        public ActionResult Edit(uint id)
        {
            var pessoa = _pessoaService.Get(id);

            if (pessoa == null || pessoa.Papel != "S")
            {
                return NotFound();
            }

            var solicitanteViewModel =
                _mapper.Map<SolicitanteViewModel>(pessoa);

            return View(solicitanteViewModel);
        }

        // POST: SolicitanteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            uint id,
            SolicitanteViewModel solicitanteViewModel)
        {
            if (id != solicitanteViewModel.Id)
            {
                return BadRequest();
            }

            // O Status não é editado pelo formulário.
            ModelState.Remove(nameof(SolicitanteViewModel.Status));

            if (!ModelState.IsValid)
            {
                return View(solicitanteViewModel);
            }

            var pessoa = _pessoaService.Get(id);

            if (pessoa == null || pessoa.Papel != "S")
            {
                return NotFound();
            }

            pessoa.Nome = solicitanteViewModel.Nome;
            pessoa.Email = solicitanteViewModel.Email;
            pessoa.Telefone = solicitanteViewModel.Telefone;
            pessoa.DataNascimento = solicitanteViewModel.DataNascimento;
            pessoa.Cep = solicitanteViewModel.Cep;
            pessoa.Cidade = solicitanteViewModel.Cidade;
            pessoa.Rua = solicitanteViewModel.Rua;
            pessoa.Numero = solicitanteViewModel.Numero;
            pessoa.Complemento = solicitanteViewModel.Complemento;

            _pessoaService.Edit(pessoa);

            return RedirectToAction(nameof(Index));
        }


        // GET: SolicitanteController/Delete/5
        public ActionResult Delete(uint id)
        {
            var pessoa = _pessoaService.Get(id);

            if (pessoa == null || pessoa.Papel != "S")
            {
                return NotFound();
            }

            var solicitanteViewModel =
                _mapper.Map<SolicitanteViewModel>(pessoa);

            return View(solicitanteViewModel);
        }

        // POST: SolicitanteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, IFormCollection collection)
        {
            var pessoa = _pessoaService.Get(id);

            if (pessoa == null || pessoa.Papel != "S")
            {
                return NotFound();
            }

            // Desativação lógica: altera Status para "I".
            _pessoaService.Delete(id);

            return RedirectToAction(nameof(Index));
        }

    }
}
