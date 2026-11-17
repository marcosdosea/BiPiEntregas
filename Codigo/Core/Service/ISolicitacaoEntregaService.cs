using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    internal interface ISolicitacaoEntregaService
    {
        uint Create(Solicitacaoentrega solicitacaoEntrega);
        void Edit(Solicitacaoentrega solicitacaoEntrega);
        void Delete(uint id);
        Solicitacaoentrega? Get(uint id);
        IEnumerable<Solicitacaoentrega> GetAll();
        decimal CalculateShipping(decimal peso);
        void Assess(uint id, byte nota, string? comentario);
        void ReportProblem(uint id, string descricao);
    }
}
