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
        IEnumerable<Pessoa> List_Pending(string Status);
        void Approve(uint id);
        void Reject(uint id);



    }
}
