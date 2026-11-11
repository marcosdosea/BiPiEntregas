using BipiEntregasWeb.Models;
using Core;
using AutoMapper;
using Mapster;

namespace BibliotecaWeb.Mapper
{
    public class AutorMapper
    {
        public static PessoaViewModel ToViewModel(Pessoa pessoa)
        {
            return pessoa.Adapt<PessoaViewModel>();
        }

        public static Pessoa ToEntity(PessoaViewModel pessoaViewModel)
        {
            return pessoaViewModel.Adapt<Pessoa>();
        }

        public static IEnumerable<PessoaViewModel> ToViewModel(IEnumerable<Pessoa> pessoas)
        {
            return pessoas.Adapt<IEnumerable<PessoaViewModel>>();
        }

        public static IEnumerable<Pessoa> ToEntity(IEnumerable<PessoaViewModel> pessoasViewModel)
        {
            return pessoasViewModel.Adapt<IEnumerable<Pessoa>>();
        }
    }
}