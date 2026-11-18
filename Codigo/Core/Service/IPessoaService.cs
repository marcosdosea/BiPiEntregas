using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IPessoaService
    {
        uint Create(Pessoa pessoa);
        void Edit(Pessoa pessoa);
        void Delete(uint id);
        Pessoa? Get(uint id);
        IEnumerable<Pessoa> GetAll();
        IEnumerable<Pessoa> GetAll(string status);
        void Activate(uint id);

        void ApproveRegistration(uint id);
        void RejectRegistration(uint id, string motivo);
    }
}
