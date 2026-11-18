using AutoMapper;
using BipiEntregasWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BipiEntregasWeb.Controllers
{
    public class EntregadorController : Controller
    {

        // Controller utiliza os métodos do PessoaService para consultar,
        // cadastrar, editar e desativar pessoas do tipo Entregador (Papel = E).
        // O Entregador é um usuário que realiza entregas.

        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;

        public EntregadorController(
            IPessoaService pessoaService,
            IMapper mapper)
        {
            _pessoaService = pessoaService;
            _mapper = mapper;
        }



        // GET: EntregadorController
        public ActionResult Index()
        {
            var entregadores = _pessoaService.GetAll()
                .Where(p => p.Papel == "E")
                .ToList();

            var entregadoresViewModel =
                _mapper.Map<List<EntregadorViewModel>>(entregadores);

            return View(entregadoresViewModel);
        }

        // GET: EntregadorController/Details/5
        public ActionResult Details(uint id)
        {
            var entregador = _pessoaService.Get(id);

            if (entregador == null || entregador.Papel != "E")
            {
                return NotFound();
            }

            var entregadorViewModel =
                _mapper.Map<EntregadorViewModel>(entregador);

            return View(entregadorViewModel);
        }

        // GET: EntregadorController/Create
        public ActionResult Create()
        {
            var entregadorViewModel = new EntregadorViewModel();
            return View(entregadorViewModel);
        }

        // POST: EntregadorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EntregadorViewModel entregadorViewModel)
        {
            if (entregadorViewModel.DataNascimento == default ||
                entregadorViewModel.DataNascimento.Date > DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(entregadorViewModel.DataNascimento),
                    "Informe uma data de nascimento válida.");
            }

            byte[]? documentoVeiculo = null;

            if (entregadorViewModel.DocumentoVeiculo is null ||
                entregadorViewModel.DocumentoVeiculo.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(entregadorViewModel.DocumentoVeiculo),
                    "Envie o documento do veículo.");
            }
            else
            {
                await using var memoryStream = new MemoryStream();
                await entregadorViewModel.DocumentoVeiculo.CopyToAsync(memoryStream);
                documentoVeiculo = memoryStream.ToArray();
            }

            // O Status é definido automaticamente pelo sistema.
            ModelState.Remove(nameof(EntregadorViewModel.Status));

            if (!ModelState.IsValid)
            {
                return View(entregadorViewModel);
            }

            var entregador = new Pessoa
            {
                Nome = entregadorViewModel.Nome,
                Cpf = entregadorViewModel.Cpf,
                Email = entregadorViewModel.Email,
                Telefone = entregadorViewModel.Telefone,
                DataNascimento = entregadorViewModel.DataNascimento,
                Cnh = entregadorViewModel.Cnh,
                Veiculo = entregadorViewModel.Veiculo,
                Placa = entregadorViewModel.Placa,
                DocumentoVeiculo = documentoVeiculo,

                Papel = "E",
                Status = "A"
            };

            try
            {
                _pessoaService.Create(entregador);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível cadastrar o entregador. " +
                    "Verifique os dados e se CPF ou e-mail já estão cadastrados.");

                return View(entregadorViewModel);
            }
        }

        // GET: EntregadorController/Edit/5
        public ActionResult Edit(uint id)
        {
            var entregador = _pessoaService.Get(id);

            if (entregador == null || entregador.Papel != "E")
            {
                return NotFound();
            }

            var entregadorViewModel =
                _mapper.Map<EntregadorViewModel>(entregador);

            return View(entregadorViewModel);
        }


        // POST: EntregadorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(uint id, 
            EntregadorViewModel entregadorViewModel)
        {
          
                if (id != entregadorViewModel.Id)
                {
                    return BadRequest();
                }

                // O Status não é editado pelo formulário.
                ModelState.Remove(nameof(EntregadorViewModel.Status));

                if (!ModelState.IsValid)
                {
                    return View(entregadorViewModel);
                }

                var entregador = _pessoaService.Get(id);

                if (entregador == null || entregador.Papel != "E")
                {
                    return NotFound();
                }

            entregador.Nome = entregadorViewModel.Nome;
            entregador.Cpf = entregadorViewModel.Cpf;
            entregador.Email = entregadorViewModel.Email;
            entregador.Telefone = entregadorViewModel.Telefone;
            entregador.DataNascimento = entregadorViewModel.DataNascimento;
            entregador.Cnh = entregadorViewModel.Cnh;
            entregador.Veiculo = entregadorViewModel.Veiculo;
            entregador.Placa = entregadorViewModel.Placa;
            if (entregadorViewModel.DocumentoVeiculo is not null &&
                entregadorViewModel.DocumentoVeiculo.Length > 0)
            {
                await using var memoryStream = new MemoryStream();
                await entregadorViewModel.DocumentoVeiculo.CopyToAsync(memoryStream);
                entregador.DocumentoVeiculo = memoryStream.ToArray();
            }

            _pessoaService.Edit(entregador);

                return RedirectToAction(nameof(Index));
            }

        // GET: EntregadorController/Delete/5
        public ActionResult Delete(uint id)
        {
            var entregador = _pessoaService.Get(id);

            if (entregador == null || entregador.Papel != "E")
            {
                return NotFound();
            }

            var entregadorViewModel =
                _mapper.Map<EntregadorViewModel>(entregador);

            return View(entregadorViewModel);
        }

        // POST: EntregadorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, IFormCollection collection)
        {
            var entregador = _pessoaService.Get(id);

            if (entregador == null || entregador.Papel != "E")
            {
                return NotFound();
            }

            // Desativação lógica: altera Status para "I".
            _pessoaService.Delete(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
