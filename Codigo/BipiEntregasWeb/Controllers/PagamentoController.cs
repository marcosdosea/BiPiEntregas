using AutoMapper;
using BipiEntregasWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BipiEntregasWeb.Controllers
{
    public class PagamentoController : Controller
    {
        // Controller utiliza os métodos do PagamentoService para consultar,
        // cadastrar, editar e estornar pagamentos das solicitações de entrega.
        // O Status e as datas são definidos automaticamente pelo Service.

        private readonly IPagamentoService _pagamentoService;
        private readonly IMapper _mapper;

        public PagamentoController(
            IPagamentoService pagamentoService,
            IMapper mapper)
        {
            _pagamentoService = pagamentoService;
            _mapper = mapper;
        }

        // GET: PagamentoController
        public ActionResult Index()
        {
            var pagamentos = _pagamentoService.GetAll();

            var pagamentosViewModel =
                _mapper.Map<List<PagamentoViewModel>>(pagamentos);

            return View(pagamentosViewModel);
        }

        // GET: PagamentoController/Details/5
        public ActionResult Details(uint id)
        {
            var pagamento = _pagamentoService.Get(id);

            if (pagamento == null)
            {
                return NotFound();
            }

            var pagamentoViewModel =
                _mapper.Map<PagamentoViewModel>(pagamento);

            return View(pagamentoViewModel);
        }

        // GET: PagamentoController/Create
        public ActionResult Create()
        {
            var pagamentoViewModel = new PagamentoViewModel();
            return View(pagamentoViewModel);
        }

        // POST: PagamentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PagamentoViewModel pagamentoViewModel)
        {
            // O Status é definido automaticamente pelo sistema.
            ModelState.Remove(nameof(PagamentoViewModel.Status));

            if (!ModelState.IsValid)
            {
                return View(pagamentoViewModel);
            }

            var pagamento = new Pagamento
            {
                IdSolicitacaoEntrega = pagamentoViewModel.IdSolicitacaoEntrega,
                Valor = pagamentoViewModel.Valor,
                Tipo = pagamentoViewModel.Tipo
            };

            try
            {
                _pagamentoService.Create(pagamento);
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(pagamentoViewModel);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível cadastrar o pagamento. " +
                    "Verifique os dados e se o código da solicitação existe.");

                return View(pagamentoViewModel);
            }
        }

        // GET: PagamentoController/Edit/5
        public ActionResult Edit(uint id)
        {
            var pagamento = _pagamentoService.Get(id);

            if (pagamento == null)
            {
                return NotFound();
            }

            var pagamentoViewModel =
                _mapper.Map<PagamentoViewModel>(pagamento);

            return View(pagamentoViewModel);
        }

        // POST: PagamentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(uint id,
            PagamentoViewModel pagamentoViewModel)
        {
            if (id != pagamentoViewModel.Id)
            {
                return BadRequest();
            }

            // O Status não é editado pelo formulário.
            ModelState.Remove(nameof(PagamentoViewModel.Status));

            if (!ModelState.IsValid)
            {
                return View(pagamentoViewModel);
            }

            var pagamento = _pagamentoService.Get(id);

            if (pagamento == null)
            {
                return NotFound();
            }

            // A solicitação de entrega não é alterada na edição.
            pagamento.Valor = pagamentoViewModel.Valor;
            pagamento.Tipo = pagamentoViewModel.Tipo;

            try
            {
                // Só é permitido enquanto o pagamento estiver pendente.
                _pagamentoService.Edit(pagamento);
            }
            catch (Exception ex) when (ex is InvalidOperationException
                                       || ex is ArgumentException)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(pagamentoViewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: PagamentoController/Delete/5
        public ActionResult Delete(uint id)
        {
            var pagamento = _pagamentoService.Get(id);

            if (pagamento == null)
            {
                return NotFound();
            }

            var pagamentoViewModel =
                _mapper.Map<PagamentoViewModel>(pagamento);

            return View(pagamentoViewModel);
        }

        // POST: PagamentoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, IFormCollection collection)
        {
            var pagamento = _pagamentoService.Get(id);

            if (pagamento == null)
            {
                return NotFound();
            }

            try
            {
                // Estorno lógico: altera Status para "E" (CSU04 - A1).
                _pagamentoService.Delete(id);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                var pagamentoViewModel =
                    _mapper.Map<PagamentoViewModel>(pagamento);

                return View(pagamentoViewModel);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}