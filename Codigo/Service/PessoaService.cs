
using System;
using System.Collections.Generic;
using System.Linq;
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

        // Criar uma nova pessoa e registrar seus perfis.
        public uint Create(Pessoa pessoa)
        {
            // Registra o perfil informado no cadastro.
            if (!pessoa.PessoaPerfis.Any(p => p.Papel == pessoa.Papel))
            {
                pessoa.PessoaPerfis.Add(new PessoaPerfil
                {
                    Papel = pessoa.Papel,
                    Status = pessoa.Status
                });
            }

            // Todo entregador também possui o perfil de solicitante.
            if (pessoa.Papel == "E" &&
                !pessoa.PessoaPerfis.Any(p => p.Papel == "S"))
            {
                pessoa.PessoaPerfis.Add(new PessoaPerfil
                {
                    Papel = "S",
                    Status = "A"
                });
            }

            _context.Pessoas.Add(pessoa);
            _context.SaveChanges();

            return pessoa.Id;
        }

        // Permite que um solicitante também se cadastre como entregador.
        public void SolicitarPerfilEntregador(
            uint idPessoa,
            string cnh,
            string veiculo,
            string placa,
            byte[] documentoVeiculo)
        {
            // Localiza a pessoa e seus perfis cadastrados.
            var pessoa = _context.Pessoas
                .Include(p => p.PessoaPerfis)
                .FirstOrDefault(p => p.Id == idPessoa);

            if (pessoa == null)
            {
                throw new KeyNotFoundException(
                    "Pessoa não encontrada.");
            }

            // Somente solicitantes ativos podem solicitar o perfil.
            if (!pessoa.PessoaPerfis.Any(p =>
                p.Papel == "S" && p.Status == "A"))
            {
                throw new InvalidOperationException(
                    "É necessário ser um solicitante ativo.");
            }

            // Impede a duplicação do perfil de entregador.
            if (pessoa.PessoaPerfis.Any(p => p.Papel == "E"))
            {
                throw new InvalidOperationException(
                    "Esta pessoa já possui um cadastro de entregador.");
            }

            // Valida os dados obrigatórios.
            if (string.IsNullOrWhiteSpace(cnh) ||
                string.IsNullOrWhiteSpace(veiculo) ||
                string.IsNullOrWhiteSpace(placa) ||
                documentoVeiculo == null ||
                documentoVeiculo.Length == 0)
            {
                throw new ArgumentException(
                    "Informe os dados e o documento do veículo.");
            }

            // Reaproveita o cadastro existente.
            pessoa.Cnh = cnh.Trim();
            pessoa.Veiculo = veiculo.Trim();
            pessoa.Placa = placa.Trim();
            pessoa.DocumentoVeiculo = documentoVeiculo;

            // Adiciona somente o perfil de entregador.
            pessoa.PessoaPerfis.Add(new PessoaPerfil
            {
                Papel = "E",
                Status = "P"
            });

            _context.SaveChanges();
        }

        // Desativar uma pessoa existente.
        public void Delete(uint id)
        {
            var pessoa = _context.Pessoas.Find(id);

            if (pessoa != null)
            {
                pessoa.Status = "I";
                _context.SaveChanges();
            }
        }

        // Editar uma pessoa existente.
        public void Edit(Pessoa pessoa)
        {
            _context.Pessoas.Update(pessoa);
            _context.SaveChanges();
        }

        // Ativar uma pessoa existente.
        public void Activate(uint id)
        {
            var pessoa = _context.Pessoas.Find(id);

            if (pessoa != null)
            {
                pessoa.Status = "A";
                _context.SaveChanges();
            }
        }

        // Buscar uma pessoa pelo ID.
        public Pessoa? Get(uint id)
        {
            return _context.Pessoas.Find(id);
        }

        // Listar todas as pessoas.
        public IEnumerable<Pessoa> GetAll()
        {
            return _context.Pessoas
                .AsNoTracking()
                .ToList();
        }

        // Listar pessoas pelo status geral.
        // Mantido para compatibilidade com o código existente.
        public IEnumerable<Pessoa> GetAll(string status)
        {
            return _context.Pessoas
                .AsNoTracking()
                .Where(p => p.Status == status)
                .ToList();
        }

        // Consultar entregadores pendentes de aprovação.
        public IEnumerable<Pessoa> GetPendingDeliveryRegistrations()
        {
            return _context.Pessoas
                .AsNoTracking()
                .Where(pessoa => pessoa.PessoaPerfis.Any(perfil =>
                    perfil.Papel == "E" && perfil.Status == "P"))
                .ToList();
        }

        // Aprovar somente o perfil de entregador.
        public void ApproveRegistration(uint id)
        {
            var pessoa = GetPendingDeliveryPerson(id);

            var perfilEntregador = pessoa.PessoaPerfis
                .First(p => p.Papel == "E" && p.Status == "P");

            perfilEntregador.Status = "A";

            // Limpa o motivo de rejeição anterior.
            pessoa.MotivoRejeicao = null;

            _context.SaveChanges();
        }

        // Rejeitar somente o perfil de entregador.
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

            var perfilEntregador = pessoa.PessoaPerfis
                .First(p => p.Papel == "E" && p.Status == "P");

            perfilEntregador.Status = "I";

            // Registra o motivo da rejeição.
            pessoa.MotivoRejeicao = motivoTratado;

            _context.SaveChanges();
        }

        // Localizar uma pessoa com perfil de entregador pendente.
        private Pessoa GetPendingDeliveryPerson(uint id)
        {
            var pessoa = _context.Pessoas
                .Include(p => p.PessoaPerfis)
                .FirstOrDefault(p => p.Id == id);

            if (pessoa is null)
            {
                throw new KeyNotFoundException(
                    "Cadastro não encontrado.");
            }

            // Verifica especificamente o perfil de entregador.
            var entregadorPendente = pessoa.PessoaPerfis.Any(p =>
                p.Papel == "E" && p.Status == "P");

            if (!entregadorPendente)
            {
                throw new InvalidOperationException(
                    "Esta pessoa não possui um cadastro de entregador pendente.");
            }

            return pessoa;
        }
    }
}
