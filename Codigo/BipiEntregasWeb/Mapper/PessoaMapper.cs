using AutoMapper;
using Core;
using BipiEntregasWeb.Models;

namespace BipiEntregasWeb.Mapper
{
    public class PessoaMapper : Profile
    {
        public PessoaMapper()
        {
            CreateMap<Pessoa, PessoaViewModel>();
        }
    }
}