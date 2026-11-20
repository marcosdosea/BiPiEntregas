using System.Collections.Generic;

namespace Core.Service
{
    public interface ISolicitacaoEntregaService
    {
        uint Create(Solicitacaoentrega solicitacaoEntrega);
        void Edit(Solicitacaoentrega solicitacaoEntrega);
        void Delete(uint id);
        Solicitacaoentrega? Get(uint id);
        IEnumerable<Solicitacaoentrega> GetAll();

        decimal CalculateShipping(decimal peso);
        void Assess(uint id, byte nota, string? comentario);
        void ReportProblem(uint id, string descricao);

        // CSU05 - AcompanharEntrega

        // Consultar as entregas de um solicitante.
        IEnumerable<Solicitacaoentrega> GetBySolicitante(
            uint idSolicitante);

        // Consultar uma entrega específica, verificando
        // se pertence ao solicitante informado.
        Solicitacaoentrega? GetByIdAndSolicitante(
            uint idSolicitacao,
            uint idSolicitante);
    }
}