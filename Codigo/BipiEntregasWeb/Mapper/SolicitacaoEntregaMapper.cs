using AutoMapper;
using Core;
using BipiEntregasWeb.Models;

namespace BipiEntregasWeb.Mapper
{
    public class SolicitacaoEntregaMapper : Profile
    {
        public SolicitacaoEntregaMapper()
        {
            CreateMap<Solicitacaoentrega, SolicitacaoEntregaViewModel>();
        }
    }
}