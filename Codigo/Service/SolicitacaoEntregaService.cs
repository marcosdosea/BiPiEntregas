
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class SolicitacaoEntregaService : ISolicitacaoEntregaService
    {
        private readonly BiPiEntregasContext _context;

        private const string STATUS_PENDENTE = "PENDENTE";
        private const string STATUS_CANCELADA = "CANCELADA";

        // Valores provisórios do frete.
        private const decimal TAXA_BASE_FRETE = 10.00m;
        private const decimal VALOR_POR_KG = 2.50m;

        public SolicitacaoEntregaService(BiPiEntregasContext context)
        {
            _context = context;
        }

        // CSU01 - Criar solicitação
        public uint Create(Solicitacaoentrega solicitacaoEntrega)
        {
            solicitacaoEntrega.Status = STATUS_PENDENTE;
            solicitacaoEntrega.DataCriacao = DateTime.Now;
            solicitacaoEntrega.IdEntregador = null;
            solicitacaoEntrega.ValorFrete =
                CalculateShipping(solicitacaoEntrega.Peso);

            _context.Solicitacaoentregas.Add(solicitacaoEntrega);
            _context.SaveChanges();

            return solicitacaoEntrega.Id;
        }

        // CSU01 - Cancelar solicitação pendente
        public void Delete(uint id)
        {
            var solicitacaoEntrega =
                _context.Solicitacaoentregas.Find(id);

            if (solicitacaoEntrega != null)
            {
                if (solicitacaoEntrega.Status != STATUS_PENDENTE)
                {
                    throw new InvalidOperationException(
                        "Só é possível cancelar solicitações aguardando entregador.");
                }

                solicitacaoEntrega.Status = STATUS_CANCELADA;
                _context.SaveChanges();
            }
        }

        // CSU01 - Editar solicitação
        public void Edit(Solicitacaoentrega solicitacaoEntrega)
        {
            if (solicitacaoEntrega.Status != STATUS_PENDENTE)
            {
                throw new InvalidOperationException(
                    "Só é possível alterar solicitações aguardando entregador.");
            }

            solicitacaoEntrega.ValorFrete =
                CalculateShipping(solicitacaoEntrega.Peso);

            _context.Solicitacaoentregas.Update(solicitacaoEntrega);
            _context.SaveChanges();
        }

        // Consulta geral por ID - compatibilidade
        public Solicitacaoentrega? Get(uint id)
        {
            return _context.Solicitacaoentregas.Find(id);
        }

        // Consulta geral - compatibilidade com Controller e testes
        public IEnumerable<Solicitacaoentrega> GetAll()
        {
            return _context.Solicitacaoentregas
                .AsNoTracking()
                .ToList();
        }

        // CSU05 - Consultar entregas de um solicitante
        public IEnumerable<Solicitacaoentrega> GetBySolicitante(
            uint idSolicitante)
        {
            return _context.Solicitacaoentregas
                .AsNoTracking()
                .Where(s => s.IdSolicitante == idSolicitante)
                .OrderByDescending(s => s.DataCriacao)
                .ToList();
        }

        // CSU05 - Consultar detalhes de uma entrega
        // pertencente ao solicitante informado
        public Solicitacaoentrega? GetByIdAndSolicitante(
            uint idSolicitacao,
            uint idSolicitante)
        {
            return _context.Solicitacaoentregas
                .AsNoTracking()
                .FirstOrDefault(s =>
                    s.Id == idSolicitacao &&
                    s.IdSolicitante == idSolicitante);
        }

        // CSU01 - Calcular frete
        public decimal CalculateShipping(decimal peso)
        {
            if (peso <= 0)
            {
                throw new ArgumentException(
                    "O peso deve ser maior que zero.",
                    nameof(peso));
            }

            return Math.Round(
                TAXA_BASE_FRETE + (peso * VALOR_POR_KG), 2);
        }

        // CSU05 - Avaliar entrega
        // Pendente de estrutura no banco de dados
        public void Assess(uint id, byte nota, string? comentario)
        {
            throw new NotImplementedException(
                "Avaliação ainda não disponível: faltam as colunas no banco de dados.");
        }

        // CSU05 - Reportar problema
        // Pendente de estrutura no banco de dados
        public void ReportProblem(uint id, string descricao)
        {
            throw new NotImplementedException(
                "Registro de problema ainda não disponível: falta a coluna no banco de dados.");
        }
    }
}
