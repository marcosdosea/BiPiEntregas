using AutoMapper;
using BipiEntregasWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core;
using Microsoft.EntityFrameworkCore;

namespace BipiEntregasWeb.Controllers
{
    public class SolicitacaoEntregaController : Controller
    {
        // Controller utiliza os métodos do SolicitacaoEntregaService para consultar,
        // cadastrar, editar e cancelar solicitações de entrega.
        // Status, data de criação e valor do frete são definidos pelo Service.

        private readonly ISolicitacaoEntregaService _solicitacaoEntregaService;
        private readonly IMapper _mapper;

        public SolicitacaoEntregaController(
            ISolicitacaoEntregaService solicitacaoEntregaService,
            IMapper mapper)
        {
            _solicitacaoEntregaService = solicitacaoEntregaService;
            _mapper = mapper;
        }

        // GET: SolicitacaoEntregaController
        public ActionResult Index()
        {
            var solicitacoes = _solicitacaoEntregaService.GetAll();

            var solicitacoesViewModel =
                _mapper.Map<List<SolicitacaoEntregaViewModel>>(solicitacoes);

            return View(solicitacoesViewModel);
        }

        // GET: SolicitacaoEntregaController/Details/5
        public ActionResult Details(uint id)
        {
            var solicitacaoEntrega = _solicitacaoEntregaService.Get(id);

            if (solicitacaoEntrega == null)
            {
                return NotFound();
            }

            var solicitacaoEntregaViewModel =
                _mapper.Map<SolicitacaoEntregaViewModel>(solicitacaoEntrega);

            return View(solicitacaoEntregaViewModel);
        }

        // GET: SolicitacaoEntregaController/Create
        public ActionResult Create()
        {
            var solicitacaoEntregaViewModel = new SolicitacaoEntregaViewModel();

            return View(solicitacaoEntregaViewModel);
        }

        // POST: SolicitacaoEntregaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SolicitacaoEntregaViewModel solicitacaoEntregaViewModel)
        {
            // O Status é definido automaticamente pelo sistema.
            // Não é necessário recebê-lo do formulário.
            ModelState.Remove(nameof(SolicitacaoEntregaViewModel.Status));

            if (!ModelState.IsValid)
            {
                return View(solicitacaoEntregaViewModel);
            }

            var solicitacaoEntrega = new Solicitacaoentrega
            {
                IdSolicitante = solicitacaoEntregaViewModel.IdSolicitante,
                RuaOrigem = solicitacaoEntregaViewModel.RuaOrigem,
                NumeroOrigem = solicitacaoEntregaViewModel.NumeroOrigem,
                BairroOrigem = solicitacaoEntregaViewModel.BairroOrigem,
                CidadeOrigem = solicitacaoEntregaViewModel.CidadeOrigem,
                CepOrigem = solicitacaoEntregaViewModel.CepOrigem,
                RuaDestino = solicitacaoEntregaViewModel.RuaDestino,
                NumeroDestino = solicitacaoEntregaViewModel.NumeroDestino,
                BairroDestino = solicitacaoEntregaViewModel.BairroDestino,
                CidadeDestino = solicitacaoEntregaViewModel.CidadeDestino,
                CepDestino = solicitacaoEntregaViewModel.CepDestino,
                DescricaoItem = solicitacaoEntregaViewModel.DescricaoItem,
                Peso = solicitacaoEntregaViewModel.Peso,
                Dimensoes = solicitacaoEntregaViewModel.Dimensoes
            };

            try
            {
                _solicitacaoEntregaService.Create(solicitacaoEntrega);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível cadastrar a solicitação. " +
                    "Verifique os dados e se o código do solicitante existe.");

                return View(solicitacaoEntregaViewModel);
            }
        }

        // GET: SolicitacaoEntregaController/Edit/5
        public ActionResult Edit(uint id)
        {
            var solicitacaoEntrega = _solicitacaoEntregaService.Get(id);

            if (solicitacaoEntrega == null)
            {
                return NotFound();
            }

            var solicitacaoEntregaViewModel =
                _mapper.Map<SolicitacaoEntregaViewModel>(solicitacaoEntrega);

            return View(solicitacaoEntregaViewModel);
        }

        // POST: SolicitacaoEntregaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            uint id,
            SolicitacaoEntregaViewModel solicitacaoEntregaViewModel)
        {
            if (id != solicitacaoEntregaViewModel.Id)
            {
                return BadRequest();
            }

            // O Status não é editado pelo formulário.
            ModelState.Remove(nameof(SolicitacaoEntregaViewModel.Status));

            if (!ModelState.IsValid)
            {
                return View(solicitacaoEntregaViewModel);
            }

            var solicitacaoEntrega = _solicitacaoEntregaService.Get(id);

            if (solicitacaoEntrega == null)
            {
                return NotFound();
            }

            // O solicitante não é alterado na edição.
            solicitacaoEntrega.RuaOrigem = solicitacaoEntregaViewModel.RuaOrigem;
            solicitacaoEntrega.NumeroOrigem = solicitacaoEntregaViewModel.NumeroOrigem;
            solicitacaoEntrega.BairroOrigem = solicitacaoEntregaViewModel.BairroOrigem;
            solicitacaoEntrega.CidadeOrigem = solicitacaoEntregaViewModel.CidadeOrigem;
            solicitacaoEntrega.CepOrigem = solicitacaoEntregaViewModel.CepOrigem;
            solicitacaoEntrega.RuaDestino = solicitacaoEntregaViewModel.RuaDestino;
            solicitacaoEntrega.NumeroDestino = solicitacaoEntregaViewModel.NumeroDestino;
            solicitacaoEntrega.BairroDestino = solicitacaoEntregaViewModel.BairroDestino;
            solicitacaoEntrega.CidadeDestino = solicitacaoEntregaViewModel.CidadeDestino;
            solicitacaoEntrega.CepDestino = solicitacaoEntregaViewModel.CepDestino;
            solicitacaoEntrega.DescricaoItem = solicitacaoEntregaViewModel.DescricaoItem;
            solicitacaoEntrega.Peso = solicitacaoEntregaViewModel.Peso;
            solicitacaoEntrega.Dimensoes = solicitacaoEntregaViewModel.Dimensoes;

            try
            {
                // Só é permitido enquanto a solicitação aguarda entregador.
                _solicitacaoEntregaService.Edit(solicitacaoEntrega);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(solicitacaoEntregaViewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: SolicitacaoEntregaController/Delete/5
        public ActionResult Delete(uint id)
        {
            var solicitacaoEntrega = _solicitacaoEntregaService.Get(id);

            if (solicitacaoEntrega == null)
            {
                return NotFound();
            }

            var solicitacaoEntregaViewModel =
                _mapper.Map<SolicitacaoEntregaViewModel>(solicitacaoEntrega);

            return View(solicitacaoEntregaViewModel);
        }

        // POST: SolicitacaoEntregaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, IFormCollection collection)
        {
            var solicitacaoEntrega = _solicitacaoEntregaService.Get(id);

            if (solicitacaoEntrega == null)
            {
                return NotFound();
            }

            try
            {
                // Cancelamento lógico: altera Status para "CANCELADA".
                _solicitacaoEntregaService.Delete(id);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                var solicitacaoEntregaViewModel =
                    _mapper.Map<SolicitacaoEntregaViewModel>(solicitacaoEntrega);

                return View(solicitacaoEntregaViewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Acompanhar()
        {
            var idPessoa = HttpContext.Session.GetInt32("IdPessoa");

            if (idPessoa == null || idPessoa <= 0)
            {
                return RedirectToAction("Login", "Pessoa");
            }

            if (HttpContext.Session.GetString("PerfilAtivo") != "S")
            {
                return StatusCode(403);
            }

            var solicitacoes = _solicitacaoEntregaService
                .GetBySolicitante((uint)idPessoa.Value);

            var model = _mapper
                .Map<List<SolicitacaoEntregaViewModel>>(solicitacoes);

            return View("Acompanhar", model);
        }

        // CSU05 - Consultar detalhes de uma entrega
        [HttpGet]
        public ActionResult AcompanharDetalhes(uint id)
        {
            var idPessoa = HttpContext.Session.GetInt32("IdPessoa");

            if (idPessoa == null || idPessoa <= 0)
            {
                return RedirectToAction("Login", "Pessoa");
            }

            if (HttpContext.Session.GetString("PerfilAtivo") != "S")
            {
                return StatusCode(403);
            }

            if (id == 0)
            {
                return BadRequest();
            }

            var solicitacao = _solicitacaoEntregaService
                .GetByIdAndSolicitante(
                    id,
                    (uint)idPessoa.Value);

            if (solicitacao == null)
            {
                return NotFound();
            }

            var model = _mapper
                .Map<SolicitacaoEntregaViewModel>(solicitacao);

            return View("AcompanharDetalhes", model);
        }
    }
}