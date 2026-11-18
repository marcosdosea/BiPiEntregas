using System;
using System.Collections.Generic;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PagamentoService : IPagamentoService
    {
        private readonly BiPiEntregasContext _context;

        public PagamentoService(BiPiEntregasContext context)
        {
            _context = context;
        }

        // Criar um novo pagamento (status inicial: P = Pendente)
        public uint Create(Pagamento pagamento)
        {
            ValidatePayment(pagamento);

            pagamento.Status = "P";
            pagamento.DataPagamento = null;
            pagamento.DataEstorno = null;

            _context.Pagamentos.Add(pagamento);
            _context.SaveChanges();
            return pagamento.Id;
        }

        // Estornar um pagamento existente (E = Estornado)
        // CSU04 - A1 EstornarPagamento: o registro é mantido no banco.
        public void Delete(uint id)
        {
            var pagamento = _context.Pagamentos.Find(id);
            if (pagamento != null)
            {
                if (pagamento.Status != "P" && pagamento.Status != "A")
                {
                    throw new InvalidOperationException(
                        "Somente pagamentos pendentes ou aprovados podem ser estornados.");
                }

                pagamento.Status = "E";
                pagamento.DataEstorno = DateTime.Now;
                _context.SaveChanges();
            }
        }

        // Editar um pagamento existente (somente se estiver pendente)
        public void Edit(Pagamento pagamento)
        {
            if (pagamento.Status != "P")
            {
                throw new InvalidOperationException(
                    "Somente pagamentos pendentes podem ser alterados.");
            }

            ValidatePayment(pagamento);

            _context.Pagamentos.Update(pagamento);
            _context.SaveChanges();
        }

        // Pegar um pagamento pelo ID
        public Pagamento? Get(uint id)
        {
            return _context.Pagamentos.Find(id);
        }

        // Listar todos os pagamentos
        public IEnumerable<Pagamento> GetAll()
        {
            return _context.Pagamentos
                .AsNoTracking()
                .ToList();
        }

        // Validar os dados do pagamento (CSU04 - "Sistema valida e salva")
        // Tipo: P = Pix, C = Cartão
        private static void ValidatePayment(Pagamento pagamento)
        {
            if (pagamento.Valor <= 0)
            {
                throw new ArgumentException(
                    "O valor do pagamento deve ser maior que zero.",
                    nameof(pagamento));
            }

            if (pagamento.Tipo != "P" && pagamento.Tipo != "C")
            {
                throw new ArgumentException(
                    "A forma de pagamento deve ser Pix ou Cartão.",
                    nameof(pagamento));
            }
        }
    }
}