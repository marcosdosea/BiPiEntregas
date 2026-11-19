using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BipiEntregasWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BipiEntregasWeb.Controllers
{
    public class EntregadorController : Controller
    {
        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;

        public EntregadorController(IPessoaService pessoaService, IMapper mapper)
        {
            _pessoaService = pessoaService;
            _mapper = mapper;
        }

        // Compatibilidade: reconhece os cadastros antigos (Papel = E)
        // e os perfis de entregador da nova tabela pessoaperfil.
        private static bool PossuiPerfilEntregador(Pessoa pessoa)
        {
            return pessoa.Papel == "E" ||
                   pessoa.PessoaPerfis.Any(perfil => perfil.Papel == "E");
        }

        // GET: Entregador/Index
        public ActionResult Index()
        {
            var entregadores = _pessoaService.GetAll()
                .Where(PossuiPerfilEntregador)
                .ToList();

            var models = _mapper.Map<List<EntregadorViewModel>>(entregadores);
            return View(models);
        }

        // GET: Entregador/Details/5
        public ActionResult Details(uint id)
        {
            var pessoa = _pessoaService.Get(id);
            if (pessoa == null || !PossuiPerfilEntregador(pessoa))
                return NotFound();

            return View(_mapper.Map<EntregadorViewModel>(pessoa));
        }

        // O cadastro de entregador agora começa com dados pessoais e endereço
        // no formulário compartilhado de solicitante.
        // GET: Entregador/Create
        public ActionResult Create()
        {
            return RedirectToAction("Create", "Solicitante");
        }

        // Rota antiga mantida para compatibilidade de assinatura.
        // Não cria mais Pessoa incompleta (sem CEP/endereço) nem CPF duplicado.
        // POST: Entregador/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> Create(EntregadorViewModel entregadorViewModel)
        {
            return Task.FromResult<ActionResult>(
                RedirectToAction("Create", "Solicitante"));
        }

        // GET: Entregador/ConcluirCadastro
        [HttpGet]
        public IActionResult ConcluirCadastro()
        {
            var idSessao = HttpContext.Session.GetString("CadastroEntregadorPessoaId");
            if (!uint.TryParse(idSessao, out uint idPessoa))
                return RedirectToAction("Create", "Solicitante");

            var pessoa = _pessoaService.Get(idPessoa);
            if (pessoa == null || pessoa.Papel != "S" || pessoa.Status != "A")
            {
                HttpContext.Session.Remove("CadastroEntregadorPessoaId");
                return RedirectToAction("Create", "Solicitante");
            }

            return View(new DocumentacaoEntregadorViewModel());
        }

        // POST: Entregador/ConcluirCadastro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConcluirCadastro(
            DocumentacaoEntregadorViewModel model)
        {
            var idSessao = HttpContext.Session.GetString("CadastroEntregadorPessoaId");
            if (!uint.TryParse(idSessao, out uint idPessoa))
                return RedirectToAction("Create", "Solicitante");

            const long tamanhoMaximo = 5 * 1024 * 1024;
            var arquivo = model.DocumentoVeiculo;

            if (arquivo == null || arquivo.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(model.DocumentoVeiculo), "Envie o documento do veículo.");
            }
            else
            {
                if (arquivo.Length > tamanhoMaximo)
                {
                    ModelState.AddModelError(
                        nameof(model.DocumentoVeiculo), "O documento deve ter no máximo 5 MB.");
                }

                var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
                if (extensao != ".pdf" && extensao != ".jpg" &&
                    extensao != ".jpeg" && extensao != ".png")
                {
                    ModelState.AddModelError(
                        nameof(model.DocumentoVeiculo), "Envie PDF, JPG ou PNG.");
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await using var memoria = new MemoryStream();
                await arquivo!.CopyToAsync(memoria);

                // A pessoa já existe: somente os dados do entregador
                // e seu perfil pendente são adicionados pelo Service.
                _pessoaService.SolicitarPerfilEntregador(
                    idPessoa,
                    model.Cnh,
                    model.Veiculo,
                    model.Placa,
                    memoria.ToArray());

                HttpContext.Session.Remove("CadastroEntregadorPessoaId");
                return RedirectToAction(nameof(CadastroConcluido));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty, "Não foi possível salvar o perfil de entregador.");
            }

            return View(model);
        }

        // GET: Entregador/CadastroConcluido
        [HttpGet]
        public IActionResult CadastroConcluido()
        {
            return View();
        }

        // GET: Entregador/Edit/5
        public ActionResult Edit(uint id)
        {
            var pessoa = _pessoaService.Get(id);
            if (pessoa == null || !PossuiPerfilEntregador(pessoa))
                return NotFound();

            return View(_mapper.Map<EntregadorViewModel>(pessoa));
        }

        // POST: Entregador/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(uint id, EntregadorViewModel entregadorViewModel)
        {
            if (id != entregadorViewModel.Id)
                return BadRequest();

            ModelState.Remove(nameof(EntregadorViewModel.Status));
            // A substituição do documento é opcional durante a edição.
            ModelState.Remove(nameof(EntregadorViewModel.DocumentoVeiculo));

            if (!ModelState.IsValid)
                return View(entregadorViewModel);

            var pessoa = _pessoaService.Get(id);
            if (pessoa == null || !PossuiPerfilEntregador(pessoa))
                return NotFound();

            pessoa.Nome = entregadorViewModel.Nome;
            pessoa.Cpf = entregadorViewModel.Cpf;
            pessoa.Email = entregadorViewModel.Email;
            pessoa.Telefone = entregadorViewModel.Telefone;
            pessoa.DataNascimento = entregadorViewModel.DataNascimento;
            pessoa.Cnh = entregadorViewModel.Cnh;
            pessoa.Veiculo = entregadorViewModel.Veiculo;
            pessoa.Placa = entregadorViewModel.Placa;

            var arquivo = entregadorViewModel.DocumentoVeiculo;
            if (arquivo != null && arquivo.Length > 0)
            {
                if (arquivo.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(entregadorViewModel.DocumentoVeiculo),
                        "O documento deve ter no máximo 5 MB.");
                    return View(entregadorViewModel);
                }

                var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
                if (extensao != ".pdf" && extensao != ".jpg" &&
                    extensao != ".jpeg" && extensao != ".png")
                {
                    ModelState.AddModelError(nameof(entregadorViewModel.DocumentoVeiculo),
                        "Envie PDF, JPG ou PNG.");
                    return View(entregadorViewModel);
                }

                await using var memoria = new MemoryStream();
                await arquivo.CopyToAsync(memoria);
                pessoa.DocumentoVeiculo = memoria.ToArray();
            }

            _pessoaService.Edit(pessoa);
            return RedirectToAction(nameof(Index));
        }

        // GET: Entregador/Delete/5
        public ActionResult Delete(uint id)
        {
            var pessoa = _pessoaService.Get(id);
            if (pessoa == null || !PossuiPerfilEntregador(pessoa))
                return NotFound();

            return View(_mapper.Map<EntregadorViewModel>(pessoa));
        }

        // POST: Entregador/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, IFormCollection collection)
        {
            var pessoa = _pessoaService.Get(id);
            if (pessoa == null || !PossuiPerfilEntregador(pessoa))
                return NotFound();

            // A operação legada Delete(id) desativa a Pessoa inteira.
            // Por segurança, não a usamos para pessoas com perfil S + E.
            // A desativação isolada de E será implementada no Service.
            if (pessoa.Papel == "S")
                return Conflict("A desativação isolada do perfil de entregador ainda precisa ser implementada.");

            _pessoaService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
