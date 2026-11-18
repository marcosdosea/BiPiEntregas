using BipiEntregasWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace BipiEntregasWeb.Controllers
{
    public class AdministradorController : Controller
    {
        private readonly IPessoaService _pessoaService;

        public AdministradorController(IPessoaService pessoaService)
        {
            _pessoaService = pessoaService;
        }

        [HttpGet]
        public ActionResult CadastrosPendentes()
        {
            var cadastros = _pessoaService.GetAll("P")
                .Where(pessoa => pessoa.Papel == "E")
                .Select(pessoa => new CadastrosPendentesViewModel
                {
                    Id = pessoa.Id,
                    Nome = pessoa.Nome,
                    Cpf = pessoa.Cpf,
                    Email = pessoa.Email,
                    Veiculo = pessoa.Veiculo,
                    Placa = pessoa.Placa,
                    Cnh = pessoa.Cnh,
                    PossuiDocumentoVeiculo =
                        pessoa.DocumentoVeiculo is { Length: > 0 }
                })
                .ToList();

            return View(cadastros);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aprovar(uint id)
        {
            try
            {
                _pessoaService.ApproveRegistration(id);
                TempData["Sucesso"] = "Cadastro aprovado.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Erro"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["Erro"] = ex.Message;
            }

            return RedirectToAction(nameof(CadastrosPendentes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rejeitar(
            uint id,
            RejeitarCadastroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Erro"] = "Informe um motivo válido para a rejeição.";
                return RedirectToAction(nameof(CadastrosPendentes));
            }

            try
            {
                _pessoaService.RejectRegistration(id, model.Motivo);
                TempData["Sucesso"] = "Cadastro rejeitado.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Erro"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["Erro"] = ex.Message;
            }
            catch (ArgumentException ex)
            {
                TempData["Erro"] = ex.Message;
            }

            return RedirectToAction(nameof(CadastrosPendentes));
        }
    }
}