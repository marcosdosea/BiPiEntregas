using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class SolicitacaoEntregaService : ISolicitacaoEntregaService
    {
        // Status da solicitação (enum da coluna status no banco)
     
        private const string STATUS_PENDENTE = "PENDENTE";
        private const string STATUS_CANCELADA = "CANCELADA";

        // Valores provisórios do frete (RF10 prevê cálculo por distância)
        private const decimal TAXA_BASE_FRETE = 10.00m;
        private const decimal VALOR_POR_KG = 2.50m;

        private readonly BiPiEntregasContext _context;

        public SolicitacaoEntregaService(BiPiEntregasContext context)
        {
            _context = context;
        }

        // Criar uma nova solicitação de entrega (status inicial: Aguardando Entregador)
        public uint Create(Solicitacaoentrega solicitacaoEntrega)
        {
            solicitacaoEntrega.Status = STATUS_PENDENTE;
            solicitacaoEntrega.DataCriacao = DateTime.Now;
            solicitacaoEntrega.IdEntregador = null;
            solicitacaoEntrega.ValorFrete = CalculateShipping(solicitacaoEntrega.Peso);

            _context.Solicitacaoentregas.Add(solicitacaoEntrega);
            _context.SaveChanges();
            return solicitacaoEntrega.Id;
        }

        // Cancelar uma solicitação existente (só se ainda estiver aguardando entregador)
        public void Delete(uint id)
        {
            var solicitacaoEntrega = _context.Solicitacaoentregas.Find(id);
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

        // Editar uma solicitação existente (só se ainda estiver aguardando entregador)
        public void Edit(Solicitacaoentrega solicitacaoEntrega)
        {
            if (solicitacaoEntrega.Status != STATUS_PENDENTE)
            {
                throw new InvalidOperationException(
                    "Só é possível alterar solicitações aguardando entregador.");
            }

            solicitacaoEntrega.ValorFrete = CalculateShipping(solicitacaoEntrega.Peso);

            _context.Solicitacaoentregas.Update(solicitacaoEntrega);
            _context.SaveChanges();
        }

        // Pegar uma solicitação pelo ID
        public Solicitacaoentrega? Get(uint id)
        {
            return _context.Solicitacaoentregas.Find(id);
        }

        // Listar todas as solicitações (com o solicitante, para exibir o nome)
        public IEnumerable<Solicitacaoentrega> GetAll()
        {
            return _context.Solicitacaoentregas
                .AsNoTracking()
                .ToList();
        }

        // Calcular o valor do frete a partir do peso (CSU01 - CalcularFrete)
        public decimal CalculateShipping(decimal peso)
        {
            if (peso <= 0)
            {
                throw new ArgumentException("O peso deve ser maior que zero.", nameof(peso));
            }

            return Math.Round(TAXA_BASE_FRETE + (peso * VALOR_POR_KG), 2);
        }

        // Avaliar uma entrega concluída com nota de 1 a 5 (CSU05 - A1, RF27)
     
        public void Assess(uint id, byte nota, string? comentario)
        {
            throw new NotImplementedException(
                "Avaliação ainda não disponível: faltam as colunas no banco de dados.");
        }

        // Reportar um problema na entrega (CSU05 - A2)
   
        public void ReportProblem(uint id, string descricao)
        {
            throw new NotImplementedException(
                "Registro de problema ainda não disponível: falta a coluna no banco de dados.");
        }
    }
}
