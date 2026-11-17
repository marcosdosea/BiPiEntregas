
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
    }
}
