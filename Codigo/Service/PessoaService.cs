using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PessoaService : IPessoaService
    {
        private readonly BiPiEntregasContext context;
        public PessoaService(BiPiEntregasContext context)
        {
            this.context = context;
        }
       
        public uint Create(Pessoa pessoa)
        {
            context.Add(pessoa);
            context.SaveChanges();
            return  pessoa.Id;
        }
        public void Edit(Pessoa pessoa)
        {
            context.Update(pessoa);
            context.SaveChanges();
        }
        public void Delete(uint id)
        {
            var pessoa = context.Pessoas.Find(id);
            if(pessoa != null)
            {
                context.Pessoas.Remove(pessoa);
                context.SaveChanges();
            }
        }
        public Pessoa? Get(uint id)
        {
           return context.Pessoas.Find(id);


        }
        public IEnumerable<Pessoa> GetAll()
        {
            return context.Pessoas.AsNoTracking();
        }
       
        public IEnumerable<Pessoa> List_Pending(string Status)
        {
            return context.Pessoas.AsNoTracking().Where(p => p.Status == Status);
        }
        public void Approve(uint id)
        {
            var pessoa = context.Pessoas.Find(id);
            if (pessoa != null)
            {
                pessoa.Status = "A";
                context.Update(pessoa);
                context.SaveChanges();
            }
        }
        public void Reject(uint id)
        {
            var pessoa = context.Pessoas.Find(id);
            if (pessoa != null)
            {
                pessoa.Status = "I";
                context.Update(pessoa);
                context.SaveChanges();
            }
        }
    
    }
}
