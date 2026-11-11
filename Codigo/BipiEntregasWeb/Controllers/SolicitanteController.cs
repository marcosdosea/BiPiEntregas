using AutoMapper;
using BipiEntregasWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace BipiEntregasWeb.Controllers
{
    public class SolicitanteController : Controller
    {
        private readonly IPessoaService _solicitanteService;
        private readonly IMapper _mapper;

        public SolicitanteController(IPessoaService solicitanteService, IMapper mapper)
        {
            _solicitanteService = solicitanteService;
            _mapper = mapper;
        }

        // GET: SolicitanteController
        public ActionResult Index()
        {
            var listaSolicitantes = _solicitanteService.GetAll();
            var listaSolicitantesModel = _mapper.Map<List<SolicitanteModel>>(listaSolicitantes);
            return View(listaSolicitantesModel);
        }

        // GET: SolicitanteController/Details/5
        public ActionResult Details(int id)
        {
            PessoaService solicitante = _solicitanteService.Get(id);
            SolicitanteModel solicitanteModel = _mapper.Map<SolicitanteModel>(solicitante);
            return View(solicitanteModel);
        }

        // GET: SolicitanteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SolicitanteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SolicitanteModel solicitanteModel)
        {
            if (!ModelState.IsValid)
                return View(solicitanteModel);

            var solicitante = _mapper.Map<SolicitanteModel>(solicitanteModel);
            _solicitanteService.Create(solicitante);
            return RedirectToAction(nameof(Index));
        }

        // GET: SolicitanteController/Edit/5
        public ActionResult Edit(int id)
        {
            SolicitanteModel solicitante = _solicitanteService.Create(id);
            SolicitanteModel solicitanteModel = _mapper.Map<SolicitanteModel>(solicitante);
            return View(solicitanteModel);
        }

        // POST: SolicitanteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, SolicitanteModel solicitanteModel)
        {
            if (!ModelState.IsValid)
                return View(solicitanteModel);

            var solicitante = _mapper.Map<SolicitanteModel>(solicitanteModel);
            _solicitanteService.Edit(solicitante);
            return RedirectToAction(nameof(Index));
        }

        // GET: SolicitanteController/Delete/5
        public ActionResult Delete(int id)
        {
            SolicitanteModel solicitante = _solicitanteService.Get(id);
            SolicitanteModel solicitanteModel = _mapper.Map<SolicitanteModel>(solicitante);
            return View(solicitanteModel);
        }

        // POST: SolicitanteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, SolicitanteModel solicitanteModel)
        {
            _solicitanteService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}