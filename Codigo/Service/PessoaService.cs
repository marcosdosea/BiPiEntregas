
using System;
using System.Collections.Generic;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PessoaService : IPessoaService
    {
        private readonly BiPiEntregasContext _context;

        public PessoaService(BiPiEntregasContext context)
        {
            _context = context;
        }

        // Criar uma nova pessoa
        public uint Create(Pessoa pessoa)
        {
            _context.Pessoas.Add(pessoa);
            _context.SaveChanges(); 
            return pessoa.Id;
        }

        // Desativar uma pessoa existente
        public void Delete(uint id)
        {
            var pessoa = _context.Pessoas.Find(id);
            if (pessoa != null)
            {
                pessoa.Status = "I";
                _context.SaveChanges();
            }
        }

        // Editar uma pessoa existente
        public void Edit(Pessoa pessoa)
        {
            _context.Pessoas.Update(pessoa);
            _context.SaveChanges();
        }

        // Ativar uma pessoa existente
        public void Activate(uint id)
        {
            var pessoa = _context.Pessoas.Find(id);

            if (pessoa != null)
            {
                pessoa.Status = "A";
                _context.SaveChanges();
            }
        }

        // Pegar uma pessoa pelo ID
        public Pessoa? Get(uint id)
        {
            return _context.Pessoas.Find(id);
        }

        public IEnumerable<Pessoa> GetAll()
        {
            return _context.Pessoas
                .AsNoTracking()
                .ToList();
        }

        // Listar todas as pessoas pelo status (A = Ativo, I = Inativo)
        public IEnumerable<Pessoa> GetAll(string status)
        {
            return _context.Pessoas
                .AsNoTracking()
                .Where(p => p.Status == status)
                .ToList();
        }
        public void ApproveRegistration(uint id)
        {
            var pessoa = GetPendingDeliveryPerson(id);

            pessoa.Status = "A";
            pessoa.MotivoRejeicao = null;

            _context.SaveChanges();
        }

        public void RejectRegistration(uint id, string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo))
            {
                throw new ArgumentException(
                    "Informe o motivo da rejeição.",
                    nameof(motivo));
            }

            var motivoTratado = motivo.Trim();

            if (motivoTratado.Length > 500)
            {
                throw new ArgumentException(
                    "O motivo deve ter no máximo 500 caracteres.",
                    nameof(motivo));
            }

            var pessoa = GetPendingDeliveryPerson(id);

            pessoa.Status = "I";
            pessoa.MotivoRejeicao = motivoTratado;

            _context.SaveChanges();
        }

        private Pessoa GetPendingDeliveryPerson(uint id)
        {
            var pessoa = _context.Pessoas.Find(id);

            if (pessoa is null)
            {
                throw new KeyNotFoundException("Cadastro não encontrado.");
            }

            if (pessoa.Papel != "E")
            {
                throw new InvalidOperationException(
                    "Somente cadastros de entregadores podem ser validados.");
            }

            if (pessoa.Status != "P")
            {
                throw new InvalidOperationException(
                    "Somente cadastros pendentes podem ser validados.");
            }

            return pessoa;
        }
    }
}
