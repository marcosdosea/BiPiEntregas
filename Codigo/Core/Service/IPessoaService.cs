using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IPessoaService
    {
    uint Create(Pessoa pessoa);
        void SolicitarPerfilEntregador(
        uint idPessoa,
        string cnh,
        string veiculo,
        string placa,
        byte[] documentoVeiculo);

        void Edit(Pessoa pessoa);
        void Delete(uint id);
        Pessoa? Get(uint id);
        IEnumerable<Pessoa> GetAll();
        IEnumerable<Pessoa> GetAll(string status);
        // Consultar pessoas com perfil de entregador pendente
        IEnumerable<Pessoa> GetPendingDeliveryRegistrations();
        void Activate(uint id);

        void ApproveRegistration(uint id);
        void RejectRegistration(uint id, string motivo);
    }
}
